using HendrixRFID.Data;

namespace HendrixRFID.Services;

public class JobService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JobService> _logger;

    public JobService(IServiceScopeFactory scopeFactory, ILogger<JobService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Beregn hvor lang tid til kl. 03:00
            var now     = DateTime.Now;
            var nextRun = DateTime.Today.AddHours(3);

            // Hvis kl. 03:00 allerede er passeret i dag, kør i morgen
            if (now > nextRun)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            _logger.LogInformation("job kører næste gang om {Hours} timer og {Minutes} minutter",
                (int)delay.TotalHours, delay.Minutes);

            // Vent til kl. 03:00
            await Task.Delay(delay, stoppingToken);

            // Kør jobbet
            await RunJobsAsync();
        }
    }

    private async Task RunJobsAsync()
    {
        _logger.LogInformation("job starter kl. {Time}", DateTime.Now);

        using var scope = _scopeFactory.CreateScope();
        var locationDecision = scope.ServiceProvider.GetRequiredService<LocationDecisionService>();
        await locationDecision.RunAsync();

        _logger.LogInformation("job færdig kl. {Time}", DateTime.Now);
    }
}