using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.DI;

public sealed class WpfAppBuilder
{
    private const string AppSettingsJsonFile = "appsettings.json";
    
    private readonly ServiceCollection _services = new();

    internal WpfAppBuilder()
    {
        Configuration = new ConfigurationManager();
        Configuration.AddJsonFile(AppSettingsJsonFile, optional: true, reloadOnChange: true);
        Configuration.AddEnvironmentVariables("WPFAPP_");

        Logging = new LoggingBuilder(Services);
        // By default, add LoggerFactory and Logger services with no providers. This way
        // when components try to get an ILogger<> from the IServiceProvider, they don't get 'null'.
        Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
        Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

        Services.AddSingleton<IConfiguration>(Configuration);

        ConfigureDefaultLogging();
    }

    public ILoggingBuilder Logging { get; }
    
    public IServiceCollection Services => _services;
    
    public ConfigurationManager Configuration { get; }

    public WpfAppBuilder UseApp<T>() where T : Application
    {
        Services.AddSingleton<Application, T>();
        return this;
    }

    public WpfAppBuilder UseWindow<T>() where T : Window
    {
        Services.AddSingleton<Window, T>();
        return this;
    }

    public WpfApp Build()
    {
        ServiceProviderOptions serviceProviderOptions = new()
        {
            ValidateOnBuild = true, 
            ValidateScopes = true
        };
        
        
        IServiceProvider provider = Services.BuildServiceProvider(serviceProviderOptions);

        WpfApp application = new(provider);
        
        return application;
    }

    private void ConfigureDefaultLogging()
    {
        Services.AddLogging(configure =>
        {
            configure.AddConfiguration(Configuration.GetSection("Logging"));
            configure.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
                options.IncludeScopes = false;
            });
            configure.AddDebug();
            configure.AddEventLog();
            configure.AddEventSourceLogger();
        });
    }

    private sealed class LoggingBuilder : ILoggingBuilder
    {
        public IServiceCollection Services { get; }

        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}