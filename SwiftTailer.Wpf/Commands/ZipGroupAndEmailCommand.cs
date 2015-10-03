using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class ZipGroupAndEmailCommand : ICommand
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

            await Task.Run(() => ZipTasks.ZipGroup(group, fileLocation.FileName, vm));

            EmailTasks.SendAttachment(
                Settings.UserEmail, 
                "Log Files", 
                Path.GetFileName(fileLocation.FileName), 
                fileLocation.FileName);
        }

        public event EventHandler CanExecuteChanged;
    }
}