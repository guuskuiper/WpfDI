using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace WinformsBuilder;

public sealed class WinformsAppBuilder
{
    private const string AppSettingsJsonFile = "appsettings.json";

    private readonly ServiceCollection _services = new();

    internal WinformsAppBuilder()
    {
        Configuration = new ConfigurationManager();
        Configuration.AddJsonFile(AppSettingsJsonFile, optional: true, reloadOnChange: true);
        Configuration.AddEnvironmentVariables("WINFORMSAPP_");

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

    public WinformsAppBuilder UseForm<T>() where T : Form
    {
        Services.AddSingleton<Form, T>();
        return this;
    }

    public WinformsApp Build()
    {
        ServiceProviderOptions serviceProviderOptions = new()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        };


        IServiceProvider provider = Services.BuildServiceProvider(serviceProviderOptions);

        WinformsApp application = new(provider);

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