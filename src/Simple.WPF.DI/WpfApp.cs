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

    /// <summary>
    /// Gets the application's dependency-injection service provider.
    /// </summary>
    public IServiceProvider Services => _services;

    /// <summary>
    /// Gets the application's configuration.
    /// </summary>
    public IConfiguration Configuration => _services.GetRequiredService<IConfiguration>();

    /// <summary>
    /// Gets the application's host environment.
    /// </summary>
    public IHostEnvironment Environment => _services.GetRequiredService<IHostEnvironment>();

    /// <summary>
    /// Gets the application's logger.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Creates a WPF application builder with default configuration.
    /// </summary>
    public static WpfAppBuilder CreateBuilder() => new();

    /// <summary>
    /// Creates a WPF application builder with the specified command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static WpfAppBuilder CreateBuilder(string[] args) => new(args);

    /// <summary>
    /// Creates a WPF application builder with the specified settings.
    /// </summary>
    /// <param name="settings">The settings used to configure the application builder.</param>
    public static WpfAppBuilder CreateBuilder(WpfAppBuilderSettings settings) => new WpfAppBuilder(settings);

    /// <summary>
    /// Runs the WPF application and displays the configured main window.
    /// </summary>
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
