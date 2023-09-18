using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.DI;

public sealed class WpfAppBuilder
{
    private readonly ServiceCollection _services = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfAppBuilder"/> class with preconfigured defaults.
    /// </summary>
    /// <remarks>
    ///   The following defaults are applied to the returned <see cref="WpfAppBuilder"/>:
    ///   <list type="bullet">
    ///     <item><description>set the <see cref="IHostEnvironment.ContentRootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/></description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from "WPFAPP_" prefixed environment variables</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from environment variables</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from supplied command line args</description></item>
    ///     <item><description>configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output</description></item>
    ///     <item><description>enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'</description></item>
    ///   </list>
    /// </remarks>
    /// <param name="args">The command line args.</param>
	internal WpfAppBuilder(string[]? args)
    {
        Configuration = new ConfigurationManager();

        AddHostConfiguration(Configuration);
        IHostEnvironment hostEnvironment = CreateHostEnvironment(Configuration);
        Services.AddSingleton(hostEnvironment);

        Configuration.AddEnvironmentVariables("WPFAPP_");
		Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		Configuration.AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true);
		Configuration.AddEnvironmentVariables();
		Configuration.AddCommandLine(args ?? Array.Empty<string>());

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
		ServiceProviderOptions serviceProviderOptions =
			Environments.Development.Equals(Configuration[HostDefaults.EnvironmentKey], StringComparison.OrdinalIgnoreCase)
				? new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }
				: new ServiceProviderOptions();


		IServiceProvider provider = Services.BuildServiceProvider(serviceProviderOptions);

        WpfApp application = new(provider);
        
        return application;
    }

    /// <summary>
    /// Set the Configuration key/value for HostDefaults.EnvironmentKey, HostDefaults.ApplicationKey and HostDefaults.ContentRootKey
    /// </summary>
    private static void AddHostConfiguration(IConfiguration configuration)
    {
	    string environmentName = Environments.Production;
	    Assembly executingAssembly = Assembly.GetExecutingAssembly();
	    AssemblyConfigurationAttribute? assemblyConfigurationAttribute = executingAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
	    if (assemblyConfigurationAttribute != null)
	    {
		    environmentName = assemblyConfigurationAttribute.Configuration == "Debug"
			    ? Environments.Development
			    : Environments.Production;
	    }
	    configuration[HostDefaults.EnvironmentKey] = environmentName;
	    configuration[HostDefaults.ApplicationKey] = executingAssembly.GetName().Name;
	    configuration[HostDefaults.ContentRootKey] = AppContext.BaseDirectory;
    }

    private static IHostEnvironment CreateHostEnvironment(IConfiguration configuration)
    {
	    IHostEnvironment hostEnvironment = new HostEnvironment
	    {
		    ApplicationName = configuration[HostDefaults.ApplicationKey] ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown",
		    EnvironmentName = configuration[HostDefaults.EnvironmentKey] ?? Environments.Production,
		    ContentRootPath = configuration[HostDefaults.ContentRootKey] ?? AppContext.BaseDirectory,
	    };

	    IFileProvider fileProvider = new PhysicalFileProvider(hostEnvironment.ContentRootPath);
	    hostEnvironment.ContentRootFileProvider = fileProvider;

	    return hostEnvironment;
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

	private sealed class HostEnvironment : IHostEnvironment
	{
		public string EnvironmentName { get; set; } = string.Empty;
		public string ApplicationName { get; set; } = string.Empty;
		public string ContentRootPath { get; set; } = string.Empty;
		public IFileProvider ContentRootFileProvider { get; set; } = null!;
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