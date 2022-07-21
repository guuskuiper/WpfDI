using System;
using Microsoft.Extensions.DependencyInjection;
using WPFDI.ViewModels;

namespace WPFDI;

public static class MainProgram
{
    [STAThread]
    public static void Main()
    {
        var builder = WpfApp.CreateBuilder();
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.UseApp<App>();
        builder.Build();
    }
}