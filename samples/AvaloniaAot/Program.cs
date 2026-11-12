using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaView;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Avalonia.Hosting;
using ViewModels;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<WpfAppOptions>(builder.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));
builder.Services.AddSingleton<MainViewModel>();

AppBuilder BuildAvaloniaApp(IServiceProvider services) =>
    AppBuilder.Configure(() => new App(services))
        .UsePlatformDetect()
        .WithInterFont()
        .AfterSetup(app =>
        {
            if (app.Instance?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = services.GetRequiredService<Window>();
                window.DataContext = services.GetRequiredService<MainViewModel>();
                desktop.MainWindow = window;
            }
        });

builder.AddAvaloniaDesktopHost<MainWindow>(BuildAvaloniaApp);

using var host = builder.Build();
host.Run();
