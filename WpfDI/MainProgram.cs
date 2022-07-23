using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WPFDI.ViewModels;

namespace WPFDI;

public static class MainProgram
{
    [STAThread]
    public static void Main()
    {
        var builder = WpfApp.CreateBuilder();

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