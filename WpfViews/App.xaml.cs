using System.Windows;

namespace WpfViews
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App(MainWindow mainWindow)
        {
            mainWindow.Show();
        }
    }
}