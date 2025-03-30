# Simple.WPF.Hosting
[![.NET](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/dt/Simple.WPF.Hosting.svg)](https://www.nuget.org/packages/Simple.WPF.Hosting)
[![NuGet](https://img.shields.io/nuget/vpre/Simple.WPF.Hosting.svg)](https://www.nuget.org/packages/Simple.WPF.Hosting)


Add WPF as a HostedService to a Host from Microsoft.Extensions.Hosting.

## Install

``` dotnet add package Simple.WPF.Hosting``` (not yet available, get the source from https://github.com/guuskuiper/WpfDI/tree/hosting)

Since WPF is Windows only, you need a .Net-windows project.

## Usage

Top-level Program.cs:
```csharp
using Microsoft.Extensions.Hosting;
using Simple.WPF.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.AddWPFHost<App, MainWindow>();
var app = builder.Build();
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
Compared to the Simple.WPF.DI package, the Host automatically starts HostedServices.
