using System.Windows;
using Microsoft.Extensions.Hosting;

namespace WpfHosting;

internal class MyApp : IHost
{
    private readonly Application _app;

    public MyApp(Application app, IServiceProvider services)
    {
        _app = app;
        Services = services;
    }

    public void Dispose()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken = new ())
    {
        _app.Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = new ())
    {
        _app.Shutdown();
        return Task.CompletedTask;
    }

    public IServiceProvider Services { get; }
}