using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace Fievel.Wpf.Commands
{
    public class SaveToTextCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var content = parameter as string;

            var fileDiag = new SaveFileDialog
            {
                FileName = $"LogCapture_{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("HHmmss")}.txt",
                Filter = ".txt|Text File|.log|Log File"
            };

            var result = fileDiag.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            File.WriteAllText(fileDiag.FileName, content);

            // wait for the OS to register the new file for 5 seconds
            var stopTime = DateTime.Now.AddSeconds(5);
            while (!File.Exists(fileDiag.FileName) && DateTime.Now < stopTime)
            {
                await Task.Delay(500);
            }

            if (!File.Exists(fileDiag.FileName))
            {
                MessageBox.Show(
                    $"{fileDiag.FileName} does not exist. It appears the file export failed. Dunno why. Sorry!", "File not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // open explorer and browse to the file
            var args = $"/e, /select, \"{fileDiag.FileName}\"";

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
