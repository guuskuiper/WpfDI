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

builder.AddAvaloniaDesktopHost<MainWindow>(BuildAvaloniaAppFromServiceProvider);
var app = builder.Build();

app.Run();

sealed partial class Program
{
    /// <summary>
    /// Only used by the visual designer in <see cref="BuildAvaloniaApp"/>
    /// </summary>
    private static readonly IServiceProvider EmptyServiceProvider = new ServiceCollection().BuildServiceProvider();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain / Setup is called: things aren't initialized
    // yet and stuff might break.

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once UnusedMember.Global
    public static AppBuilder BuildAvaloniaApp() => BuildAvaloniaAppFromServiceProvider(EmptyServiceProvider);

    private static AppBuilder BuildAvaloniaAppFromServiceProvider(IServiceProvider serviceProvider)
        => AppBuilder.Configure(() => new App(serviceProvider))
            .UsePlatformDetect()
            .WithInterFont()
            .AfterSetup(builder =>
            {
                // The ApplicationLifetime is null when using the previewer.
                if (builder.Instance?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    AfterDesktopSetup(desktop, serviceProvider);
                }
            });

    private static void AfterDesktopSetup(IClassicDesktopStyleApplicationLifetime desktop, IServiceProvider serviceProvider)
    {
        Window mainWindow = serviceProvider.GetRequiredService<Window>();
        MainViewModel mainWindowViewModel = serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainWindowViewModel;
        desktop.MainWindow = mainWindow;
    }
}
