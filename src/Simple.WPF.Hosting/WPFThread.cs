using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.WPF.Hosting;

internal sealed class WPFThread
{
    private readonly IServiceProvider _provider;
    private readonly Thread _uiThread;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public WPFThread(IServiceProvider provider, IHostApplicationLifetime applicationLifetime)
    {
        _provider = provider;
        _applicationLifetime = applicationLifetime;
        _uiThread = new Thread(ThreadStart)
        {
            Name = "WPF Thread",
            IsBackground = true,
        };
        _uiThread.SetApartmentState(ApartmentState.STA);
    }

    public void Start() => _uiThread.Start();

    public void Stop() => Application.Current?.Dispatcher.InvokeAsync(Shutdown);

    private void Shutdown() => Application.Current?.Shutdown();

    private void ThreadStart()
    {
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

        Application app = _provider.GetRequiredService<Application>();

        if (app.StartupUri is not null)
        {
            MessageBox.Show("Please remove the StartupUri configuration in App.xaml");
            HandleApplicationExit();
            return;
        }
        Window window = _provider.GetRequiredService<Window>();

        app.Exit += (sender, e) => HandleApplicationExit();

        app.Run(window);
    }

    private void HandleApplicationExit()
    {
        _applicationLifetime.StopApplication();
    }
}
