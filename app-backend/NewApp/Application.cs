using Twenty57.Builder.ApplicationRuntime.Interfaces;

namespace NewApp;

internal sealed class Application(IEnumerable<IService> services, ILogger<Application> logger) : IHostedService
{
    private readonly IEnumerable<IService> _services = services;
    private readonly ILogger<Application> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application started.");
        foreach (var service in _services)
        {
            await service.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application stopping.");
        foreach (var service in _services)
        {
            await service.StopAsync(cancellationToken);
        }
    }
}
