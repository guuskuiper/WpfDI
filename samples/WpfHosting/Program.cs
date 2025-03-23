using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.WPF.Hosting;
using ViewModels;
using WpfViews;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton<MainViewModel>();
builder.Services.Configure<WpfAppOptions>(builder.Configuration.GetRequiredSection(WpfAppOptions.WpfApp));

builder.AddWPFHost<App, MainWindow>();
var app = builder.Build();
app.Run();
