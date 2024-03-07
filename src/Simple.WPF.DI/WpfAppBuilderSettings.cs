using Microsoft.Extensions.Configuration;
namespace Simple.WPF.DI;

public sealed class WpfAppBuilderSettings
{
    /// <summary>
    /// The command line arguments to add to the <see cref="WpfAppBuilder.Configuration"/>.
    /// </summary>
    public string[]? Args { get; set; }
    
    /// <summary>
    /// Initial configuration sources to be added to the <see cref="WpfAppBuilder.Configuration"/>. These sources can influence
    /// the <see cref="WpfAppBuilder.Environment"/> through the use of <see cref="Microsoft.Extensions.Hosting.HostDefaults"/> keys.
    /// </summary>
    public ConfigurationManager? Configuration { get; set; }

    /// <summary>
    /// The environment name.
    /// </summary>
    public string? EnvironmentName { get; set; }

    /// <summary>
    /// The application name.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// The content root path.
    /// </summary>
    public string? ContentRootPath { get; set; }
}