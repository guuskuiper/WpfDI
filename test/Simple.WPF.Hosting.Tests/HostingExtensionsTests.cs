using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.WPF.Hosting.Tests;

public class HostingExtensionsTests
{
    [Fact]
    public void AddWPFHost_RegistersWPFThreadAndHostedService()
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());

        builder.AddWPFHost<TestApplication, TestWindow>();

        var app = builder.Build();

        var wpfThread = app.Services.GetService<WPFThread>();
        var hostedService = app.Services.GetService<IHostedService>();

        Assert.NotNull(wpfThread);
        Assert.NotNull(hostedService);
        Assert.IsType<WPFHostedService>(hostedService);
    }

    private class TestApplication : Application { }
    private class TestWindow : Window { }
}
