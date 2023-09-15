using System.Windows;
using Microsoft.Extensions.Logging;
using ViewModels;

namespace WpfViews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ILogger<MainWindow> _logger;

        public MainWindow(MainViewModel mainViewModel, ILogger<MainWindow> logger)
        {
            InitializeComponent();
            _logger = logger;
            _mainViewModel = mainViewModel;
            DataContext = mainViewModel;
            
            _logger.LogInformation("Mainwindow created");
        }
    }
}