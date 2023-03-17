using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using WpfViewModels;
using WpfBuilder;
using WpfViews;

namespace WpfDI;

public static class MainProgram
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = WpfApp.CreateBuilder();

        builder.Configuration.AddCommandLine(args);

        builder.Logging.AddNLog(CreateLoggingConfiguration());

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.Configure<WpfAppOptions>(builder.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));
        
        var app = builder
            .UseApp<App>()
            .UseWindow<MainWindow>()
            .Build();
        
        app.Logger.LogInformation("Starting WPF app");
        app.Run();
        app.Logger.LogInformation("Closed WPF app");
    }

    private static LoggingConfiguration CreateLoggingConfiguration()
    {
        LoggingConfiguration config = new LoggingConfiguration();

#if DEBUG
        string layout = "${longdate}|${pad:padding=2:inner=${threadid}}|${pad:padding=5:inner=${level:uppercase=true}}|${logger}:${callsite-linenumber}|${message}|${exception:format=tostring}";
#else
            string layout = "${longdate}|${pad:padding=2:inner=${threadid}}|${pad:padding=5:inner=${level:uppercase=true}}||${message}|${exception:format=tostring}";
#endif

        // Targets where to log to: File and Console
        FileTarget logFile = new FileTarget("logfile")
        {
            FileName = Path.Combine("WpfDI-log"),
            ArchiveFileName = Path.Combine("WpfDI-log-archive"),
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveNumbering = ArchiveNumberingMode.Date,
            MaxArchiveDays = 30,
            Layout = layout,
        };

#if DEBUG
        ConsoleTarget logConsole = new ConsoleTarget("logConsole")
        {
            Layout = layout
        };

        // Rules for mapping loggers to targets
        config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logConsole);
        config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logFile);
#else
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logFile);
#endif

        return config;
    }
}