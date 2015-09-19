using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf.Commands
{
    public class ExportLogConfigCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var fileName = Path.GetFileName(LogSource.Instance.LogFileLocation);
            var saveLocation = new SaveFileDialog
            {
                AddExtension = true,
                Filter = ".json | JSON File",
                FileName = fileName,
                Title = "Export log config to..."
            };

            var result = saveLocation.ShowDialog();
            if (!result.HasValue || !result.Value || string.IsNullOrEmpty(saveLocation.FileName))
                return;

            // save the file
            File.Copy(LogSource.Instance.LogFileLocation, saveLocation.FileName);

            // browse to it in explorer
            var args = $"/e, /select, \"{saveLocation.FileName}\"";

            var info = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = args
            };
            Process.Start(info);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ImportLogConfigCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var importFile = new OpenFileDialog
            {
                Title = "Choose config file..."
            };

            var result = importFile.ShowDialog();
            if (!result.HasValue || !result.Value || string.IsNullOrEmpty(importFile.FileName))
                return;

            // try to deserialize in order to validate it
            Logs logs;
            try
            {
                var json = File.ReadAllText(importFile.FileName);
                logs = JsonConvert.DeserializeObject<Logs>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The config file is invalid and could not be deserialized ({ex.Message}::{ex})",
                    "Invalid Config File", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }

            // decide how to import the config
            var importModeResult = MessageBox.Show(
                "Would you like to replace your current configuration?\r\nClick 'Yes' to replace\nClick 'No' to attempt a merge\nClick 'Cancel' to do nothing", 
                "Import Mode", 
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            switch (importModeResult)
            {
                case MessageBoxResult.Yes:
                    ReplaceConfig(logs, importFile.FileName);
                    break;
                case MessageBoxResult.No:
                    MergeConfig(logs);
                    break;
                case MessageBoxResult.Cancel:
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                default:
                    return;
            }
        }

        private void MergeConfig(Logs newConfig)
        {
            // TODO: this badly needs some unit tests
            foreach (var group in newConfig.Groups)
            {
                if (!LogSource.Instance.Logs.Groups
                    .Any(g => g.Name.Equals(group.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    // the group doesn't exist, so just add it
                    // this should cover the logs in the group as well
                    LogSource.Instance.AddGroup(group);
                }
                else
                {
                    // if the group does exist add any new logs to it
                    LogSource.Instance.AddLog(group, group.Logs);
                }
            }
        }

        private void ReplaceConfig(Logs newConfig, string importPath)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to replace your existing config file with {Path.GetFileName(importPath)}? This cannot be undone!",
                "Confirm Replace",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
                MessageBox.Show("Import operation was aborted", "Aborted", MessageBoxButton.OK,
                    MessageBoxImage.Information);


            LogSource.Instance.ReplaceConfig(newConfig);
            LogSource.Instance.SaveState();
        }

        public event EventHandler CanExecuteChanged;
    }
}
