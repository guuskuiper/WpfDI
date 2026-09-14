using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Avalonia.Hosting;

/// <summary>
/// Manages the Avalonia thread and application lifecycle.
/// </summary>
internal sealed class AvaloniaThread
{
    private readonly IServiceProvider _provider;
    private readonly Thread _uiThread;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly Func<AppBuilder, int> _startLifetime;
    private readonly TaskCompletionSource<object> _applicationExited = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<object> _applicationStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaThread"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="applicationLifetime">The host application lifetime.</param>
    /// <param name="startLifetime">Starts the selected Avalonia application lifetime.</param>
    public AvaloniaThread(
        IServiceProvider provider,
        IHostApplicationLifetime applicationLifetime,
        Func<AppBuilder, int> startLifetime)
    {
        _provider = provider;
        _applicationLifetime = applicationLifetime;
        _startLifetime = startLifetime;
        _uiThread = new Thread(ThreadStart)
        {
            Name = "Avalonia Thread",
            IsBackground = true,
        };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _uiThread.SetApartmentState(ApartmentState.STA);
        }
    }

    /// <summary>
    /// Starts the Avalonia thread.
    /// The task completes when <see cref="IControlledApplicationLifetime.Startup"/> event fires.
    /// </summary>
    public Task StartAsync(CancellationToken token)
    {
        _uiThread.Start();
        return _applicationStarted.Task.WaitAsync(token);
    }

    /// <summary>
    /// Stops the Avalonia thread.
    /// The task completes when <see cref="IControlledApplicationLifetime.Exit"/> event fires.
    /// </summary>
    public Task StopAsync(CancellationToken token)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime controlled)
            {
                controlled.Shutdown();
            }
        });
        return _applicationExited.Task.WaitAsync(token);
    }

    /// <summary>
    /// The entry point for the Avalonia thread.
    /// </summary>
    private void ThreadStart()
    {
        AppBuilder appBuilder = _provider.GetRequiredService<AppBuilder>();
        appBuilder.AfterSetup(builder =>
        {
            switch (builder.Instance?.ApplicationLifetime)
            {
                // IClassicDesktopStyleApplicationLifetime is a subclass of IControlledApplicationLifetime, so we don't need to handle it separately.
                case IControlledApplicationLifetime controlled:
                    controlled.Startup += OnStartup;
                    controlled.Exit += OnExit;
                    break;
                default:
                    // Fallback for other lifetimes.
                    SignalStartup();
                    break;
            }
        });
        _startLifetime(appBuilder);

        // Avalonia stopped.
        SignalExit();
    }

    private void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs eventArgs)
    {
        SignalStartup();
    }

    private void SignalStartup()
    {
        _applicationStarted.SetResult(null!);
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs eventArgs)
    {
        SignalExit();
    }

    private void SignalExit()
    {
        _applicationExited.TrySetResult(null!);
        _applicationLifetime.StopApplication();
    }
}
