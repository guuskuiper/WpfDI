using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.Hosting;

internal sealed class WPFHostedService : IHostedService
{
    private readonly WPFThread _thread;
    private readonly ILogger<WPFHostedService> _logger;

    public WPFHostedService(WPFThread thread, ILogger<WPFHostedService> logger)
    {
        _thread = thread;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _thread.Start();
        _logger.LogInformation("WPF thread started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("WPF thread stopping");
        _thread.Stop();
        return Task.CompletedTask;
    }
}
