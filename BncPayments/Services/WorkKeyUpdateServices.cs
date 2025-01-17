using BncPayments.Models;
using ClassLibrary.BncModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BncPayments.Services
{
    public class WorkKeyUpdateServices : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbEpaymentsContext _dbContext;
        private readonly ApiBncSettings _apiBncSettings;
        private readonly IEncryptionServices _encryptServices;
        private readonly ILogger<WorkKeyUpdateServices> _logger;
        private Timer _timer;

        public WorkKeyUpdateServices(IServiceProvider serviceProvider,
            ApiBncSettings apiBncSettings,
            IEncryptionServices encryptionServices,
            ILogger<WorkKeyUpdateServices> logger)
        {
            _serviceProvider = serviceProvider;
            _apiBncSettings = apiBncSettings;
            _encryptServices = encryptionServices;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            // testing var nextRunTime = now.AddMinutes(5); // Ejecutar en 5 minutos
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 0, 10, 0); // 00:10 de hoy

            // Si ya pasó la hora de ejecución de hoy, programa para mañana
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var initialDelay = nextRunTime - now;
            // testing _timer = new Timer(async _ => await UpdateWorkKey(), null, initialDelay, TimeSpan.FromMinutes(5)); // Intervalo de 5 minutos
            _timer = new Timer(async _ => await UpdateWorkKey(), null, initialDelay, TimeSpan.FromDays(1)); // Intervalo de 1 día
            _logger.LogInformation("WorkKeyUpdateServices started.");
            return Task.CompletedTask;
        }

        private async Task UpdateWorkKey()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bncServices = scope.ServiceProvider.GetRequiredService<IBncServices>();
                    var response = await bncServices.LogOn();
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                        if (result != null &&
                            result.Status.Equals("OK"))
                        {
                            /// desencriptar el result.Value
                            var decryptResult = _encryptServices.DecryptText(result.Value, _apiBncSettings.MasterKey);

                            var logOnResponse = JsonConvert.DeserializeObject<LogOnResponse>(decryptResult);

                            if (logOnResponse != null)
                            {
                                var modelWorkingKey = new WorkingKey()
                                {
                                    Key = logOnResponse.WorkingKey
                                };
                                var idWorkingKey = await CreateWorkingKey(modelWorkingKey);

                                if (idWorkingKey != 0)
                                {
                                    _logger.LogInformation("Creating working key BBDD.");
                                }
                            }
                            _logger.LogInformation("Returning Ok response.");
                            
                        }
                        _logger.LogInformation("Returning Ok response.");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to update working key.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the working key.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("WorkKeyUpdateServices stopped.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task<long> CreateWorkingKey(WorkingKey model)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _dbContext = scope.ServiceProvider.GetRequiredService<DbEpaymentsContext>();
                    _logger.LogInformation("Create working key from database.");

                    var workingsPast = await _dbContext.WorkingKeys.ToListAsync();
                    model.Version = 1;
                    model.Activo = true;

                    if (workingsPast.Any())
                    {
                        var lastWorking = workingsPast.LastOrDefault();
                        if (lastWorking != null)
                        {
                            lastWorking.Activo = false;
                            lastWorking.FechaExpiracion = DateTime.Now;
                            model.Version = lastWorking.Version + 1;

                            _dbContext.WorkingKeys.Update(lastWorking);
                        }

                        _dbContext.WorkingKeys.Add(model);
                        return await _dbContext.SaveChangesAsync();
                    }
                    _dbContext.WorkingKeys.Add(model);
                    return await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the working key.");
                return 0;
            }
        }
    }
}
