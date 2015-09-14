using System;
using System.Windows.Input;
using Fievel.Wpf.ViewModels;
using Microsoft.Win32;

namespace Fievel.Wpf.Commands
{
    public class PickLogCommand : ICommand
    {
        private readonly LogPickerDialogViewModel _vm;

        public PickLogCommand(LogPickerDialogViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var d = new OpenFileDialog();

            var result = d.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            var location = d.FileName;
            _vm.FileLocation = location;
        }

        public event EventHandler CanExecuteChanged;
    }
}