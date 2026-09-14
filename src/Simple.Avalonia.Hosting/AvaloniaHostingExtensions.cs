using System;
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
    /// <typeparam name="TWindow">The type of the main window.</typeparam>
    /// <param name="builder">The host application builder.</param>
    /// <param name="appBuilder">The application builder, also used by the previewer.</param>
    /// <returns>The updated host application builder.</returns>
    public static IHostApplicationBuilder AddAvaloniaDesktopHost<TWindow>(this IHostApplicationBuilder builder,
        Func<IServiceProvider, AppBuilder> appBuilder)
        where TWindow : Window
    {
        AddAvaloniaCore(
            builder,
            appBuilder,
            avaloniaBuilder => avaloniaBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>()));
        builder.Services.AddSingleton<Window, TWindow>();

        return builder;
    }

    /// <summary>
    /// Adds Avalonia main window to the host's service collection,
    /// and a <see cref="AppBuilder"/> to create the Avalonia application.
    /// </summary>
    /// <typeparam name="TWindow">The type of the main window.</typeparam>
    /// <param name="builder">The host application builder.</param>
    /// <param name="appBuilder">The application builder, also used by the previewer.</param>
    /// <returns>The updated host application builder.</returns>
    public static IHostApplicationBuilder AddAvaloniaDesktopHost<TWindow>(this IHostApplicationBuilder builder,
        Func<AppBuilder> appBuilder) where TWindow : Window
        => AddAvaloniaDesktopHost<TWindow>(builder, _ => appBuilder());

    /// <summary>
    /// Adds Avalonia with both a desktop window and a single-view root control.
    /// The appropriate root is selected by the active Avalonia application lifetime.
    /// Select the lifetime by providing a <c>Func&lt;AppBuilder, int&gt;</c> that starts the desired Avalonia application lifetime.
    /// The startup delegate can select the classic desktop lifetime using
    /// <see cref="M:Avalonia.ClassicDesktopStyleApplicationLifetimeExtensions.StartWithClassicDesktopLifetime(Avalonia.AppBuilder,System.String[],Avalonia.Controls.ShutdownMode)"/>,
    /// a single-view lifetime, or a platform-specific lifetime such as StartLinuxDrm in the Avalonia.LinuxFramebuffer package.
    /// </summary>
    /// <typeparam name="TWindow">The type of the desktop main window.</typeparam>
    /// <typeparam name="TView">The type of the single-view root control.</typeparam>
    /// <param name="builder">The host application builder.</param>
    /// <param name="appBuilder">The application builder, also used by the previewer.</param>
    /// <param name="startLifetime">Starts the selected Avalonia application lifetime.</param>
    /// <returns>The updated host application builder.</returns>
    public static IHostApplicationBuilder AddAvaloniaHost<TWindow, TView>(
        this IHostApplicationBuilder builder,
        Func<IServiceProvider, AppBuilder> appBuilder,
        Func<AppBuilder, int> startLifetime)
        where TWindow : Window
        where TView : UserControl
    {
        AddAvaloniaCore(builder, appBuilder, startLifetime);
        builder.Services.AddSingleton<Window, TWindow>();
        builder.Services.AddSingleton<UserControl, TView>();

        return builder;
    }

    /// <summary>
    /// Adds Avalonia with both a desktop window and a single-view root control.
    /// </summary>
    public static IHostApplicationBuilder AddAvaloniaHost<TWindow, TView>(
        this IHostApplicationBuilder builder,
        Func<AppBuilder> appBuilder,
        Func<AppBuilder, int> startLifetime)
        where TWindow : Window
        where TView : UserControl =>
        AddAvaloniaHost<TWindow, TView>(builder, _ => appBuilder(), startLifetime);

    private static void AddAvaloniaCore(
        IHostApplicationBuilder builder,
        Func<IServiceProvider, AppBuilder> appBuilder,
        Func<AppBuilder, int> startLifetime)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(appBuilder);
        ArgumentNullException.ThrowIfNull(startLifetime);

        builder.Services.AddSingleton(appBuilder);
        builder.Services.AddSingleton(startLifetime);
        builder.Services.AddSingleton<AvaloniaThread>();
        builder.Services.AddHostedService<AvaloniaHostedService>();
    }
}
