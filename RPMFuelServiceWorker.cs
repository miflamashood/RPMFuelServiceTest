using Cronos;
using Microsoft.Extensions.Options;

namespace RPMFuelService
{
    public class RPMFuelServiceWorker : BackgroundService
    {
        private readonly ILogger<RPMFuelServiceWorker> _logger;
        private readonly IRPMFuelDataProvider _rpmFuelDataProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<RPMFuelServiceConfig> _rpmFuelServiceConfig;

        public RPMFuelServiceWorker(ILogger<RPMFuelServiceWorker> logger,IRPMFuelDataProvider rpmFuelDataProvider, IServiceScopeFactory scopeFactory, IOptions<RPMFuelServiceConfig> rpmFuelServiceConfig)
        {
            // Inject the scope factory
            _scopeFactory = scopeFactory;
            _rpmFuelServiceConfig = rpmFuelServiceConfig ?? throw new ArgumentNullException(nameof(rpmFuelServiceConfig));

            _logger = logger;
            _rpmFuelDataProvider = rpmFuelDataProvider ?? throw new ArgumentNullException(nameof(rpmFuelDataProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await WaitForNextSchedule(_rpmFuelServiceConfig.Value.TaskDelay,stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                
                var rpmFuelService = scope.ServiceProvider
                    .GetRequiredService<IRPMFuelService>();
              
                var fuelData = await _rpmFuelDataProvider.ReadFuelData();
                
                if (fuelData.Count>0)  rpmFuelService.SyncRPMFuelData(fuelData);
              
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
               
             //   await Task.Delay(100000, stoppingToken);
            }
        }

        private async Task WaitForNextSchedule(string cronExpression,CancellationToken stoppingToken)
        {
            var parsedExp = CronExpression.Parse(cronExpression);
            var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
            var occurenceTime = parsedExp.GetNextOccurrence(currentUtcTime);

            var delay = occurenceTime.GetValueOrDefault() - currentUtcTime;
            _logger.LogInformation("The run is delayed for {delay}. Current time: {time}", delay, DateTimeOffset.Now);

            await Task.Delay(delay,stoppingToken);
        }
    }
}