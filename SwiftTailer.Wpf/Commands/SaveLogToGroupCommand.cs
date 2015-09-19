using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class SaveLogToGroupCommand : ICommand
    {
        private readonly LogPickerDialogViewModel _vm;

        public SaveLogToGroupCommand(LogPickerDialogViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = parameter as Window;
            if (w == null) return;
            
            // remove quotes
            var fileLocation = _vm.FileLocation.Trim('"');

            var newLog = new LogInfo(fileLocation, _vm.Alias);
            LogSource.Instance.Logs.Groups
                .First(group => group.Id.Equals(_vm.SelectedGroup.Id))
                .AddLog(newLog);

            w.Hide();
            w.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}