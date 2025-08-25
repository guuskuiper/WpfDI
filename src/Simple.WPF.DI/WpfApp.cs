using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

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

    public IHostEnvironment Environment => _services.GetRequiredService<IHostEnvironment>();

    public ILogger Logger { get; }

    public static WpfAppBuilder CreateBuilder() => new();

    public static WpfAppBuilder CreateBuilder(string[] args) => new(args);

    public static WpfAppBuilder CreateBuilder(WpfAppBuilderSettings settings) => new WpfAppBuilder(settings);

    public void Run()
    {
        Application app = _services.GetRequiredService<Application>();

        // Queue the creation and showing of the Window, such that it is delayed until after the Application is started using the Run call.
        Dispatcher.CurrentDispatcher.BeginInvoke(() =>
        {
            Window window = _services.GetRequiredService<Window>();
            window.Show();
        });
        app.Run();
    }
}
