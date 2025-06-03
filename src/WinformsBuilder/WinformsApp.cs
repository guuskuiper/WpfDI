using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace WinformsBuilder;

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
public sealed class WinformsApp
{
    private readonly IServiceProvider _services;

    internal WinformsApp(IServiceProvider services)
    {
        _services = services;

        string loggerName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? nameof(WinformsApp);
        Logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger(loggerName);
    }

    public IServiceProvider Services => _services;

    public IConfiguration Configuration => _services.GetRequiredService<IConfiguration>();

    public ILogger Logger { get; }

    public static WinformsAppBuilder CreateBuilder() => new();


    /// <summary>
    /// Run the application.
    /// To customize application configuration such as set high DPI settings or default font,
    /// see https://aka.ms/applicationconfiguration.
    /// This is executed before the form is created and the application runs the form.
    /// </summary>
    /// <param name="applicationConfigurationInitialize">Optionally pass ApplicationConfiguration.Initialize</param>
    public void Run(Action? applicationConfigurationInitialize = null)
    {
        applicationConfigurationInitialize?.Invoke();

        // construct Form
        Form form = _services.GetRequiredService<Form>();

        Application.Run(form);
    }
}
