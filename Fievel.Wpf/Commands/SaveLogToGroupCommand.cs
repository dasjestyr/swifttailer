using System;
using System.Windows;
using System.Windows.Input;
using Fievel.Wpf.Data;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
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

            var newLog = new LogInfo(_vm.FileLocation, _vm.Alias);
            _vm.SelectedGroup.AddLog(newLog);
            LogSource.Instance.SaveState();
            w.Hide();
            w.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}