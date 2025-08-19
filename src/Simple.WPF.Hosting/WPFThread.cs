using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.WPF.Hosting;

/// <summary>
/// Manages the WPF thread and application lifecycle.
/// </summary>
internal sealed class WPFThread
{
	private readonly IServiceProvider _provider;
	private readonly Thread _uiThread;
	private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly TaskCompletionSource<object> _applicationExited = new();
    private readonly TaskCompletionSource<object> _applicationStarted = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="WPFThread"/> class.
	/// </summary>
	/// <param name="provider">The service provider.</param>
	/// <param name="applicationLifetime">The application lifetime.</param>
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

	/// <summary>
	/// Starts the WPF thread.
	/// </summary>
	public Task StartAsync(CancellationToken token)
    {
        _uiThread.Start();
        return _applicationStarted.Task.WaitAsync(token);
    }

    /// <summary>
	/// Stops the WPF thread.
	/// </summary>
	public Task StopAsync(CancellationToken token)
    {
        Application.Current?.Dispatcher.InvokeAsync(Shutdown);
        return _applicationExited.Task.WaitAsync(token);
    }

    /// <summary>
	/// Shuts down the WPF application.
	/// </summary>
	private void Shutdown() => Application.Current?.Shutdown();

	/// <summary>
	/// The entry point for the WPF thread.
	/// </summary>
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

        if (app.FindResource(typeof(Window)) is Style style)
        {
            window.Style = style;
        }

        app.Startup += (_, args) => _applicationStarted.SetResult(null!);
		app.Exit += (sender, e) => HandleApplicationExit();

		app.Run(window);
	}

	/// <summary>
	/// Handles the application exit event.
	/// </summary>
	private void HandleApplicationExit()
	{
        _applicationExited.SetResult(null!);
        _applicationLifetime.StopApplication();
	}
}
