# Simple.WPF.DI
[![.NET](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/guuskuiper/WpfDI/actions/workflows/dotnet.yml)

## Install

``` dotnet add package Simple.WPF.DI```

Since WPF is Windows only, you need a .Net-windows project.

## Usage

Top-level Program.cs:
```csharp
using WpfBuilder;
using WpfViews;

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