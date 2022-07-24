using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WpfViewModels;

public class MainViewModel
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly WpfAppOptions _options;
    

    public MainViewModel(ILogger<MainViewModel> logger, IOptions<WpfAppOptions> options)
    {
        _logger = logger;
        _logger.LogInformation("Created VM");

        _options = options.Value;
    }

    public string Title => _options.Name;
    public string Content { get; set; } = "Lorem Ipsum, ....";
}