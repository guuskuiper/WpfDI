using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WpfBuilder;

public sealed class WpfAppBuilder
{
    private const string AppSettingsJsonFile = "appsettings.json";
    
    private readonly ServiceCollection _services = new();

    internal WpfAppBuilder()
    {
        Configuration = new ConfigurationManager();
        Configuration.AddJsonFile(AppSettingsJsonFile, optional: true, reloadOnChange: true);
        Configuration.AddEnvironmentVariables("WPFAPP_");

        Services.AddSingleton<IConfiguration>(Configuration);

        ConfigureDefaultLogging();
    }
    
    public IServiceCollection Services => _services;
    
    public ConfigurationManager Configuration { get; }

    public WpfAppBuilder UseApp<T>() where T : Application
    {
        Services.AddSingleton<Application, T>();
        return this;
    }

    public WpfApp Build()
    {
        ServiceProvider provider = Services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        WpfApp application = new WpfApp(provider);
        
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
}