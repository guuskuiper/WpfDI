using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.Avalonia.Hosting;

/// <summary>
/// A hosted service that manages the lifecycle of a Avalonia thread.
/// </summary>
internal sealed class AvaloniaHostedService : IHostedService
{
	private readonly AvaloniaThread _thread;
	private readonly ILogger<AvaloniaHostedService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="AvaloniaHostedService"/> class.
	/// </summary>
	/// <param name="thread">The Avalonia thread to manage.</param>
	/// <param name="logger">The logger to use for logging information.</param>
	public AvaloniaHostedService(AvaloniaThread thread, ILogger<AvaloniaHostedService> logger)
	{
		_thread = thread;
		_logger = logger;
	}

	/// <summary>
	/// Starts the Avalonia thread.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous start operation.</returns>
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await _thread.StartAsync(cancellationToken);
		_logger.LogInformation("Avalonia thread started");
	}

	/// <summary>
	/// Stops the Avalonia thread.
	/// </summary>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task that represents the asynchronous stop operation.</returns>
	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await _thread.StopAsync(cancellationToken);
		_logger.LogInformation("Avalonia thread stopped");
	}
}
