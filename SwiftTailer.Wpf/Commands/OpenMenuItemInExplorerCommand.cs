using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenMenuItemInExplorerCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var item = parameter as MenuItem;
            var file = item?.DataContext as TailFile;
            if (file == null) return;

            if (!File.Exists(file.LogInfo.Location))
            {
                MessageBox.Show(
                    $"{file.LogInfo.Location} does not exist.", "File not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var args = $"/e, /select, \"{file.LogInfo.Location}\"";

            var info = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = args
            };
            Process.Start(info);
        }

        public event EventHandler CanExecuteChanged;
    }
}
