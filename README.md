# Simple.WPF.DI
[![.NET](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/dt/Simple.WPF.DI.svg)](https://www.nuget.org/packages/Simple.WPF.DI) 
[![NuGet](https://img.shields.io/nuget/vpre/Simple.WPF.DI.svg)](https://www.nuget.org/packages/Simple.WPF.DI)

## Install

``` dotnet add package Simple.WPF.DI```

Since WPF is Windows only, you need a .Net-windows project.

## Usage

Top-level Program.cs:
```csharp
using Simple.WPF.DI;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

var builder = WpfApp.CreateBuilder();

builder.Services.AddSingleton<MainViewModel>();

var app = builder
    .UseApp<App>()
    .UseWindow<MainWindow>()
    .Build();

app.Run();
```

Inject a MainViewModel into the MainWindow:
```csharp

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        _mainViewModel = mainViewModel;
        DataContext = mainViewModel;
    }
}
```

When starting with the default WPF project template, a different entry-point is already defined on the App.  
This conflict with the top-level Program.cs which is now used as and entry point that sets up Dependency Injection and start the WPF GUI. 
Remove this by changing the "App.xml" properties to the "Page" build action. 
This prevents and error that there are 2 entry points in the application.

## Inject into UserControls

It's not possible in WPF to use contructor injection. 
One possible workaround is to create a static `ServiceProvider` in the `App` class.
```csharp
public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		ServiceProvider = serviceProvider;
	}

	public static IServiceProvider ServiceProvider { get; private set; } = default!;
}
```

Use this `ServiceProvider` in the contructor of the `UserControl`.
```csharp 
public partial class MyUserControl : UserControl
{
    private readonly MainViewModel = mainViewModel;

    public MyUserControl()
    {
        mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
```

## IHostedService
`IHostedSerice`'s that are registered are not automatically start when running the app.
These services can be started manually by first resolving them from the ServiceProvider:
```csharp
IEnumarable<IHosteedService> hostedServices = _services.GetServices<IHostedService>()
```
And starting them before calling `app.Run()`, and stopping them after `app.Run()` completed using `StartAsync(token)` and `StopAsync(token)`.
Consider creating extention methods on `WpfApp` to start / stop these hosted services.
