# Avalonia in Host
[![.NET](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml)

Add Avalonia as a HostedService to a Host from [Microsoft.Extensions.Hosting](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder)

The target of these extensions is Desktop applications.
Internally the Avalonia helper `StartWithClassicDesktopLifetime` is used to setup and start Avalonia for a Desktop application.

## Note
Set `<OutputType>Exe</OutputType>` in the `.csproj` file to let Ctrl-C exit the app.

## Install

``` dotnet add package Simple.Avalonia.Hosting```

Since WPF is Windows only, you need a .Net-windows project.

## Usage

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<MainWindowViewModel>();

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
        MainWindowViewModel mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = mainWindowViewModel;
        desktop.MainWindow = mainWindow;
    }
}
```
