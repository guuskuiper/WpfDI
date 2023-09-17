using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.DI;

/// <summary>
/// Provides methods to create a WPF application using Dependency Injection.
/// Example usage:
/// var builder = WpfApp.CreateBuilder();
/// builder.Services.AddSingleton&lt;MainViewModel&gt;();
/// var app = builder
///     .UseApp&lt;App&gt;()
///     .UseWindow&lt;MainWindow&gt;()
///     .Build();
/// </summary>
public sealed class WpfApp
{
    private readonly IServiceProvider _services;

    internal WpfApp(IServiceProvider services)
    {
        _services = services;

        string loggerName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? nameof(WpfApp);
        Logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger(loggerName);
    }
    
    public IServiceProvider Services => _services;
    
    public IConfiguration Configuration => _services.GetRequiredService<IConfiguration>();

    public ILogger Logger { get; }

    public static WpfAppBuilder CreateBuilder() => new();

    public void Run()
    {
        Application app = _services.GetRequiredService<Application>();

        // construct Window after Application such that Application Resources can be used
        Window window = _services.GetRequiredService<Window>(); 

        app.Run(window);
    }
}
