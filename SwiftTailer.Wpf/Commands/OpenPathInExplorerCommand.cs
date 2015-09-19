using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenPathInExplorerCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var path = parameter as string;
            if (path == null)
                MessageBox.Show($"Invalid parameter {parameter}. Expected string.", "Invalid parameter",
                    MessageBoxButton.OK, MessageBoxImage.Error);

            var args = $"/e, /select, \"{path}\"";

            var info = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = args
            };
            Process.Start(info);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class OpenLogConfigInExplorerCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MessageBox.Show(
                "This is easier if you just use your favorite editor. After editing the config file, you will need to restart the application!",
                "External Editor", MessageBoxButton.OK, MessageBoxImage.Information);

            StaticCommands.OpenPathInExplorerCommand.Execute(LogSource.Instance.LogFileLocation);
        }

        public event EventHandler CanExecuteChanged;
    }
}