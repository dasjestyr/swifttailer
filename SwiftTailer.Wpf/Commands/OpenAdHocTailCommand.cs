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
            var filePath = parameter as string;

            if (filePath == null)
            {
                var filePicker = new OpenFileDialog
                {
                    Title = "Choose file to tail..."
                };

                var result = filePicker.ShowDialog();
                if (!result.HasValue || !result.Value)
                    return;

                filePath = filePicker.FileName;
            }

            OpenWindow(filePath);
        }

        private void OpenWindow(string filename)
        {
            var window = new AdHocTailingWindow(filename);
            window.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
