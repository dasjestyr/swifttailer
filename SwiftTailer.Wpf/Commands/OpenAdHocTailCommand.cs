using System;
using System.Windows.Input;
using Microsoft.Win32;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenAdHocTailCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var filePicker = new OpenFileDialog
            {
                Title = "Choose file to tail..."
            };

            var result = filePicker.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            var window = new AdHocTailingWindow(filePicker.FileName);
            window.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
