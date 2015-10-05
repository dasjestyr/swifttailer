using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Models.Observable;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ViewSelectionWindow.xaml
    /// </summary>
    public partial class ViewSelectionWindow : Window
    {
        private ViewSelectionViewModel ViewModel => DataContext as ViewSelectionViewModel;

        public ViewSelectionWindow(IEnumerable<LogLine> logLines)
        {
            InitializeComponent();
            DataContext = new ViewSelectionViewModel();
            
            ViewModel.LogLines = logLines;

            Settings.SettingsChanged += ViewModel.SettingsChanged;
        }

        private void Window_Closing(object sender, EventArgs args)
        {
            Settings.SettingsChanged -= ViewModel.SettingsChanged;
            Debug.WriteLine("Unsubscribed from SettingsChanged event");
        }

        private void CompareClipboard_Click(object sender, RoutedEventArgs e)
        {
            StaticCommands.CompareToClipboardCommand.Execute(ContentBox);
        }

        private void PingIpAddress_Click(object sender, RoutedEventArgs e)
        {
            StaticCommands.PingSelectionCommand.Execute(ContentBox);
        }
    }
}
