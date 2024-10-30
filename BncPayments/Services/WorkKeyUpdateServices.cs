
namespace BncPayments.Services
{
    public class WorkKeyUpdateServices : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WorkingKeyServices _workingKeyServices;
        private Timer _timer;

        public WorkKeyUpdateServices(IServiceProvider serviceProvider,
            WorkingKeyServices workingKeyServices)
        {
            _serviceProvider = serviceProvider;
            _workingKeyServices = workingKeyServices;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 0, 10, 0).AddDays(1);
            var initialDelay = nextRunTime - now;

            _timer = new Timer(UpdateWorkKey, null, initialDelay, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void UpdateWorkKey(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bncServices = scope.ServiceProvider.GetRequiredService<IBncServices>();
                var workingKey = await bncServices.UpdateWorkingKey();

                if (!workingKey.Equals("KO"))
                {
                    _workingKeyServices.SetWorkingKey(workingKey);

                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
