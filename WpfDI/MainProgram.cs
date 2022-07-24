using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.Configure<WpfAppOptions>(builder.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));
        
        var app = builder
            .UseApp<App>()
            .Build();
        
        app.Logger.LogInformation("Starting WPF app");
        app.Run();
        app.Logger.LogInformation("Closed WPF app");
    }
}