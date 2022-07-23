using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WPFDI;

public sealed class WpfApp
{
    private readonly IServiceProvider _services;

    internal WpfApp(IServiceProvider services)
    {
        _services = services;
        
        Logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger(System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? nameof(WpfApp));
    }
    
    public IServiceProvider Services => _services;
    
    public IConfiguration Configuration => _services.GetRequiredService<IConfiguration>();

    public ILogger Logger { get; }

    public static WpfAppBuilder CreateBuilder() => new();

    public void Run()
    {
        Application app = _services.GetRequiredService<Application>();
        app.Run();
    }
}
