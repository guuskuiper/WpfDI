using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.Hosting;

/// <summary>
/// A hosted service that manages the lifecycle of a WPF thread.
/// </summary>
internal sealed class WPFHostedService : IHostedService
{
	private readonly WPFThread _thread;
	private readonly ILogger<WPFHostedService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="WPFHostedService"/> class.
	/// </summary>
	/// <param name="thread">The WPF thread to manage.</param>
	/// <param name="logger">The logger to use for logging information.</param>
	public WPFHostedService(WPFThread thread, ILogger<WPFHostedService> logger)
	{
		_thread = thread;
		_logger = logger;
	}

	/// <summary>
	/// Starts the WPF thread.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous start operation.</returns>
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_thread.Start();
		_logger.LogInformation("WPF thread started");
		return Task.CompletedTask;
	}

	/// <summary>
	/// Stops the WPF thread.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous stop operation.</returns>
	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("WPF thread stopping");
		_thread.Stop();
		return Task.CompletedTask;
	}
}
