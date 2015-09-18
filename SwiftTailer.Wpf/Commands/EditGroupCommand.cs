using System;
using System.Windows;
using System.Windows.Input;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Pages;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class EditGroupCommand : ICommand
    {
        private readonly AddGroupViewModel _vm;

        public EditGroupCommand()
        {
            
        }

        public EditGroupCommand(AddGroupViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var group = parameter as LogGroup;
            var window = parameter as Window;

            // 1st stage
            if (group != null)
            {
                var w = new EditGroupWindow(group);
                w.ShowDialog();
            }
            // 2nd stage
            else if (window != null && _vm != null)
            {
                // right now, we only have the name to change
                LogSource.Instance.UpdateGroup(_vm.Group);
                window.Hide();
                window.Close();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
