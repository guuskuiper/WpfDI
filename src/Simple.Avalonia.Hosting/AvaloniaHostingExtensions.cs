using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Simple.Avalonia.Hosting;

/// <summary>
/// Provides extension methods for configuring Avalonia applications with the generic host.
/// </summary>
public static class AvaloniaHostingExtensions
{
	/// <summary>
	/// Adds Avalonia main window to the host's service collection,
	/// and a <see cref="AppBuilder"/> to create the Avalonia application.
	/// </summary>
	/// <typeparam name="TWindows">The type of the main window.</typeparam>
	/// <param name="builder">The host application builder.</param>
	/// <param name="appBuilder">The application builder, also used by the previewer.</param>
	/// <returns>The updated host application builder.</returns>
	public static IHostApplicationBuilder AddAvaloniaDesktopHost<TWindows>(this IHostApplicationBuilder builder,
		Func<IServiceProvider, AppBuilder> appBuilder)
		where TWindows : Window
	{
		builder.Services.AddSingleton(appBuilder);
		builder.Services.AddSingleton<Window, TWindows>();
		builder.Services.AddSingleton<AvaloniaThread>();
		builder.Services.AddHostedService<AvaloniaHostedService>();

		return builder;
	}

    /// <summary>
    /// Adds Avalonia main window to the host's service collection,
    /// and a <see cref="AppBuilder"/> to create the Avalonia application.
    /// </summary>
	/// <typeparam name="TWindows">The type of the main window.</typeparam>
	/// <param name="builder">The host application builder.</param>
	/// <param name="appBuilder">The application builder, also used by the previewer.</param>
	/// <returns>The updated host application builder.</returns>
	public static IHostApplicationBuilder AddAvaloniaDesktopHost<TWindows>(this IHostApplicationBuilder builder,
		Func<AppBuilder> appBuilder) where TWindows : Window
			=> AddAvaloniaDesktopHost<TWindows>(builder, _ => appBuilder());
}
