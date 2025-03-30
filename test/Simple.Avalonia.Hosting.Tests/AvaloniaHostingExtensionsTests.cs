using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Simple.Avalonia.Hosting.Tests;

public class AvaloniaHostingExtensionsTests
{
    [Fact]
    public void AddAvaloniaHost_RegistersWPFThreadAndHostedService()
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());

        builder.AddAvaloniaDesktopHost<TestWindow>(sp => AppBuilder.Configure<TestApplication>());

        var app = builder.Build();

        var wpfThread = app.Services.GetService<AvaloniaThread>();
        var hostedService = app.Services.GetService<IHostedService>();

        Assert.NotNull(wpfThread);
        Assert.NotNull(hostedService);
        Assert.IsType<AvaloniaHostedService>(hostedService);
    }

    [Fact]
    public async Task StartStop_HostedService()
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());

        builder.AddAvaloniaDesktopHost<TestWindow>(sp => TestAppBuilder.BuildAvaloniaApp()
            .AfterSetup(appBuilder =>
            {
                if (appBuilder.Instance?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    Window mainWindow = sp.GetRequiredService<Window>();
                    desktop.MainWindow = mainWindow;
                }
            })
        );

        var app = builder.Build();

        var hostedService = app.Services.GetRequiredService<IHostedService>();

        CancellationTokenSource cts = new(TimeSpan.FromSeconds(100));
        await hostedService.StartAsync(cts.Token);
        bool? started = Dispatcher.UIThread.Invoke(() =>
        {
            Window window = app.Services.GetRequiredService<Window>();
            return window.IsVisible;
        });
        await hostedService.StopAsync(cts.Token);

        Assert.True(started);
    }

    [Fact(Skip = "Cannot test 2 Avalonia applications, without proper reset.")]
    public async Task StartStop_ApplicationLifetime()
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());

        builder.AddAvaloniaDesktopHost<TestWindow>(sp => TestAppBuilder.BuildAvaloniaApp()
            .AfterSetup(appBuilder =>
            {
                if (appBuilder.Instance?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    Window mainWindow = sp.GetRequiredService<Window>();
                    desktop.MainWindow = mainWindow;
                }
            })
        );

        var app = builder.Build();

        Task appTask = app.RunAsync();
        IHostApplicationLifetime hostApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        hostApplicationLifetime.ApplicationStarted.Register(x =>
        {
            hostApplicationLifetime.StopApplication();
        }, null);

        await appTask;
    }

    private class TestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApplication>()
            .UsePlatformDetect()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions()
            {
                UseHeadlessDrawing = false
            });
    }

    private class TestApplication : Application { }
    private class TestWindow : Window { }
}
