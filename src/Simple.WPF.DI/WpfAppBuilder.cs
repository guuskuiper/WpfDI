using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.DI;

public sealed class WpfAppBuilder : IHostApplicationBuilder
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
    internal WpfAppBuilder() : this(args: null)
    {
    }

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
    internal WpfAppBuilder(string[]? args) : this(new WpfAppBuilderSettings() { Args = args })
    {
    }

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
    /// <param name="settings"></param>
    internal WpfAppBuilder(WpfAppBuilderSettings? settings)
    {
        settings ??= new WpfAppBuilderSettings();

        Configuration = settings.Configuration ?? new ConfigurationManager();
        Properties = new Dictionary<object, object>();

        AddHostConfiguration(settings, Configuration);
        Environment = CreateHostEnvironment(Configuration);
        Services.AddSingleton(Environment);

        Configuration.AddEnvironmentVariables("WPFAPP_");
        Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration.AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", true);
        Configuration.AddEnvironmentVariables();
        Configuration.AddCommandLine(settings.Args ?? Array.Empty<string>());

        Metrics = new MetricsBuilder(Services);
        Logging = new LoggingBuilder(Services);
        // By default, add LoggerFactory and Logger services with no providers. This way
        // when components try to get an ILogger<> from the IServiceProvider, they don't get 'null'.
        Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
        Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

        Services.AddSingleton<IConfiguration>(Configuration);

        ConfigureDefaultLogging();
    }

    public ILoggingBuilder Logging { get; }
    public IMetricsBuilder Metrics { get; }

    public IServiceCollection Services => _services;

    /// <summary>
    /// Used by other DI providers like Autofac, Unity, etc.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="configure"></param>
    /// <typeparam name="TContainerBuilder"></typeparam>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory,
        Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        TContainerBuilder containerBuilder = factory.CreateBuilder(Services);
        configure?.Invoke(containerBuilder);
    }

    public IDictionary<object, object> Properties { get; }

    public IConfigurationManager Configuration { get; }

    public IHostEnvironment Environment { get; }

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
            Environments.Development.Equals(Configuration[HostDefaults.EnvironmentKey],
                StringComparison.OrdinalIgnoreCase)
                ? new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }
                : new ServiceProviderOptions();


        IServiceProvider provider = Services.BuildServiceProvider(serviceProviderOptions);

		// Mark the service collection as read-only to prevent future modifications
		_services.MakeReadOnly();
		
        WpfApp application = new(provider);

        return application;
    }

    /// <summary>
    /// Set the Configuration key/value for HostDefaults.EnvironmentKey, HostDefaults.ApplicationKey and HostDefaults.ContentRootKey
    /// </summary>
    private static void AddHostConfiguration(WpfAppBuilderSettings settings, IConfiguration configuration)
    {
        Assembly? executingAssembly = Assembly.GetEntryAssembly();
        bool isDebug = IsDebugAssembly(executingAssembly);
        string environmentName = isDebug ? Environments.Development : Environments.Production;

        configuration[HostDefaults.EnvironmentKey] = settings.EnvironmentName ?? environmentName;
        configuration[HostDefaults.ApplicationKey] = settings.ApplicationName ?? executingAssembly?.GetName().Name;
        configuration[HostDefaults.ContentRootKey] = settings.ContentRootPath ?? AppContext.BaseDirectory;
    }

    private static bool IsDebugAssembly(Assembly? executingAssembly)
    {
        AssemblyConfigurationAttribute? assemblyConfigurationAttribute =
            executingAssembly?.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return assemblyConfigurationAttribute?.Configuration == "Debug";
    }

    private static IHostEnvironment CreateHostEnvironment(IConfiguration configuration)
    {
        IHostEnvironment hostEnvironment = new HostEnvironment
        {
            ApplicationName = configuration[HostDefaults.ApplicationKey] ?? "Unknown",
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

    private sealed class MetricsBuilder : IMetricsBuilder
    {
        public IServiceCollection Services { get; }

        public MetricsBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
