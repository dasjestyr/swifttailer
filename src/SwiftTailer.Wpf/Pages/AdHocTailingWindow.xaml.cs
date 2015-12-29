using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for AdHocTailingWindow.xaml
    /// </summary>
    public partial class AdHocTailingWindow : Window
    {
        private AdHocTailingViewModel ViewModel => DataContext as AdHocTailingViewModel;

        public AdHocTailingWindow(string filePath)
        {
            InitializeComponent();
            Dispatcher.UnhandledException += DisplayUnhandledException;

            ViewModel.WindowTitle = $"Tailing {Path.GetFileName(filePath)}";
            ViewModel.SetTail(filePath);

            Settings.SettingsChanged += ViewModel.SettingsChanged;
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            Settings.SettingsChanged -= ViewModel.SettingsChanged;
            Debug.WriteLine("Unsubscribed from SettingsChanged event");
        }

        private void DisplayUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Global error catch 
            // Doesn't really allow the app to resume, but it's better than logs
            // for now
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "error.log");
            var logPath = Path.Combine(Settings.WorkingDirectory, "error.log");

            using (var sw = File.AppendText(logPath))
            {
                sw.WriteLine($"{DateTime.Now.ToLongDateString()} :: {e.Exception.Message} :: {e.Exception}");
            }

            MessageBox.Show($"e.Exception.Message :: {e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The selection gets changed throught it's SelectedItem binding
            // This old school sauce is to deal with the fact that custom 
            // behaviors just weren't having me 

            var lbx = sender as ListBox;
            if (lbx == null) return;
            
            var tail = ViewModel.TailFile;

            if (tail == null) return;

            if (tail.FollowTail)
                lbx.ScrollIntoView(lbx.SelectedItem);
        }

        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbi = sender as ListBoxItem;
            if (lbi == null) return;

            StaticCommands.OpenLogLineCommand.Execute(lbi);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.IsRunning)
            {
                ViewModel.StopTailing();
            }
        }
    }
}
