using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WPFDI;

public class WpfApp
{
    public ServiceCollection Services { get; init; } = new ();
    
    public static WpfApp CreateBuilder()
    {
        var builder = new WpfApp();
        return builder;
    }

    public WpfApp UseApp<T>() where T : Application
    {
        Services.AddSingleton<Application, T>();
        return this;
    }

    public void Build()
    {
        ServiceProvider provider = Services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        Application app = provider.GetRequiredService<Application>();
        app.Run();
    }
}
