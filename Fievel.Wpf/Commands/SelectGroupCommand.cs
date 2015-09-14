using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
{
    public class SelectGroupCommand : ICommand
    {
        private readonly MainViewModel _vm;

        public SelectGroupCommand(MainViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var param = parameter as MenuItem;
            var selectedGroup = _vm.Groups.Single(g => g.Name.Equals(param.Header as string, StringComparison.OrdinalIgnoreCase));

            _vm.StopTailing();
            _vm.SelectedGroup = selectedGroup;
        }

        public event EventHandler CanExecuteChanged;
    }
}