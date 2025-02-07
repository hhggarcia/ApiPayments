using BncPayments.Models;
using BncPayments.Services;
using ClassLibrary.BncModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;

namespace BncPayments.Jobs
{
    public class WorkKeyUpdateJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApiBncSettings _apiBncSettings;
        private readonly IEncryptionServices _encryptServices;
        private readonly ILogger<WorkKeyUpdateJob> _logger;

        public WorkKeyUpdateJob(IServiceProvider serviceProvider,
            ApiBncSettings apiBncSettings,
            IEncryptionServices encryptServices,
            ILogger<WorkKeyUpdateJob> logger)
        {
            _serviceProvider = serviceProvider;
            _apiBncSettings = apiBncSettings;
            _encryptServices = encryptServices;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
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

                        if (result != null && result.Status.Equals("OK"))
                        {
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
