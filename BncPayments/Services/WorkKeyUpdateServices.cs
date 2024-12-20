namespace BncPayments.Services
{
    public class WorkKeyUpdateServices : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WorkingKeyServices _workingKeyServices;
        private readonly ILogger<WorkKeyUpdateServices> _logger;
        private Timer _timer;

        public WorkKeyUpdateServices(IServiceProvider serviceProvider,
            WorkingKeyServices workingKeyServices,
            ILogger<WorkKeyUpdateServices> logger)
        {
            _serviceProvider = serviceProvider;
            _workingKeyServices = workingKeyServices;
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
                    var workingKey = await bncServices.UpdateWorkingKey();

                    if (!workingKey.Equals("KO"))
                    {
                        _logger.LogInformation("Updating working key.");
                        _workingKeyServices.SetWorkingKey(workingKey);
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
    }
}
