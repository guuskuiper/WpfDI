using ViewModels;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

var builder = WpfApp.CreateBuilder();
builder.Services.AddSingleton<MainViewModel>();
builder.Services.Configure<WpfAppOptions>(builder.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));

var app = builder
    .UseApp<App>()
    .UseWindow<MainWindow>()
    .Build();
        
app.Logger.LogInformation("Starting top-level WPF app");
app.Run();
app.Logger.LogInformation("Closed top-level WPF app");