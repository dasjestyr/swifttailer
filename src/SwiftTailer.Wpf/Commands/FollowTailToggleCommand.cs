using System;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    public class FollowTailToggleCommand : ICommand
    {
        private readonly ITailControl _vm;

        public FollowTailToggleCommand(ITailControl vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return _vm.SelectedTail != null;
        }

        public void Execute(object parameter)
        {
            _vm.SelectedTail.FollowTail = !_vm.SelectedTail.FollowTail;
        }

        public event EventHandler CanExecuteChanged;
    }
}