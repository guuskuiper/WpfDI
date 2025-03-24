using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.WPF.Hosting;

/// <summary>
/// Provides extension methods for configuring WPF applications with the generic host.
/// </summary>
public static class HostingExtensions
{
	/// <summary>
	/// Adds WPF application and main window to the host's service collection.
	/// </summary>
	/// <typeparam name="TApplication">The type of the WPF application.</typeparam>
	/// <typeparam name="TWindows">The type of the main window.</typeparam>
	/// <param name="builder">The host application builder.</param>
	/// <returns>The updated host application builder.</returns>
	public static IHostApplicationBuilder AddWPFHost<TApplication, TWindows>(this IHostApplicationBuilder builder)
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
