using System.Windows;
using Microsoft.Extensions.Hosting;
using WpfHosting;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddSingleton<MainWindow>();
    services.AddSingleton<MainViewModel>();
    services.Configure<WpfAppOptions>(context.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));
    services.AddSingleton<Application, App>();
    services.AddSingleton<IHost, MyApp>();
});
var app = builder.Build();
app.Run();