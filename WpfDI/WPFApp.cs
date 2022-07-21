using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WPFDI;

public class WpfApp
{
    public ServiceCollection Services { get; init; } = new ();
    private Type? ApplicationStartType { get; set; }
    
    public static WpfApp CreateBuilder()
    {
        var builder = new WpfApp();
        return builder;
    }

    public WpfApp UseApp<T>() where T : Application
    {
        Services.AddSingleton<T>();
        ApplicationStartType = typeof(T);
        return this;
    }

    public void Build()
    {
        if(ApplicationStartType is null) return;
        
        ServiceProvider provider = Services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        Application app = (Application)provider.GetRequiredService(ApplicationStartType);
        app.Run();
    }
}
