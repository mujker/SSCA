using System.Windows;

namespace SSCA
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new View.MainWindow().Show();
            base.OnStartup(e);
        }
    }
}