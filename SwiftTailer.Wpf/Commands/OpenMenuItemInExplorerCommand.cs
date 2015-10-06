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

            var fileExists = File.Exists(file.LogInfo.FullPath);
            var directoryExists = Directory.Exists(file.LogInfo.DirectoryPath);

            if (!fileExists && !directoryExists)
            {
                MessageBox.Show(
                    $"{file.LogInfo.FullPath} does not exist.", "File not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var args = fileExists
                ? $"/e, /select, \"{file.LogInfo.FullPath}\""
                : $"/e, \"{file.LogInfo.DirectoryPath}\"";

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
