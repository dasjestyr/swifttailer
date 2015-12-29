using System.Windows;
using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;

namespace SwiftTailer.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal TaskbarIcon TaskbarIcon;

        void App_Startup(object sender, StartupEventArgs e)
        {
            TaskbarIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }
    }
}
