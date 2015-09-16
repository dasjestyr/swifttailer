using System;
using System.Windows;
using System.Windows.Input;
using Fievel.Wpf.Data;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
{
    public class CreateGroupCommand : ICommand
    {
        private readonly AddGroupViewModel _vm;

        public CreateGroupCommand(AddGroupViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;

            var group = new LogGroup(_vm.GroupName);
            LogSource.Instance.Logs.AddLogGroup(group);

            window.Hide();
            window.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}
