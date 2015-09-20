using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class ZipGroupCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var vm = parameter as MainViewModel;
            if (vm == null) return;

            var group = vm.SelectedGroup;

            // get destination
            var fileLocation = new SaveFileDialog
            {
                Title = "Choose location for the zip file...",
                FileName = $"{group.Name}_Logs_{DateTime.Now.ToFileTime()}.zip",
                Filter = ".zip|ZIP File"
            };

            var result = fileLocation.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            vm.ProgressBarValue = 0;
            await Task.Run(() =>
            {
                // create the new zip file on disk
                using (var ms = new FileStream(fileLocation.FileName, FileMode.CreateNew))
                {
                    // create the archive to go into the file
                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Create))
                    {
                        var progressFactor = 100/group.Logs.Count;

                        // add each log to the archive
                        foreach (var log in group.Logs)
                        {
                            if (!File.Exists(log.Filename))
                                continue;

                            vm.ProgressText = $"Processing {log.Filename}...";

                            // create the entry
                            var entry = zip.CreateEntry(Path.GetFileName(log.Filename), CompressionLevel.Fastest);

                            // fill the entry
                            using (var entryStream = entry.Open())
                            {
                                using (
                                    var logFileStream = File.Open(log.Filename, FileMode.Open, FileAccess.Read,
                                        FileShare.ReadWrite))
                                {
                                    var fileBytes = new byte[logFileStream.Length];
                                    logFileStream.Read(fileBytes, 0, fileBytes.Length);
                                    using (var logStream = new MemoryStream(fileBytes))
                                    {
                                        logStream.CopyTo(entryStream);
                                        vm.ProgressBarValue += progressFactor;
                                    }
                                }
                            }
                        }
                    }
                }
            });

            vm.ProgressText = string.Empty;
            vm.ProgressBarValue = 0;
        }

        public event EventHandler CanExecuteChanged;
    }
}
