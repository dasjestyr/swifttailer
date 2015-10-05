using System;
using System.Windows.Input;
using Microsoft.Win32;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class PickLogCommand : ICommand
    {
        private readonly ILogTail _vm;

        public PickLogCommand(ILogTail vm)
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