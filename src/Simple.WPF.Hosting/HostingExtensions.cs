using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.WPF.Hosting;

public static class HostingExtensions
{
    public static IHostApplicationBuilder UseWPFHost<TApplication, TWindows>(this IHostApplicationBuilder builder)
        where TApplication : Application
        where TWindows : Window
    {
        builder.Services.AddSingleton<Application, TApplication>();
        builder.Services.AddSingleton<Window, TWindows>();
        builder.Services.AddSingleton<WPFThread>();
        builder.Services.AddHostedService<WPFHostedService>();

        return builder;
    }
}
