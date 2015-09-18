using System;
using System.Windows;
using System.Windows.Input;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf.Commands
{
    public class DeleteGroupCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var group = parameter as LogGroup;
            if (group == null) return;

            var confirm = MessageBox.Show($"Are you sure you want to delete the group \"{group.Name}\"?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Cancel)
                return;

            LogSource.Instance.RemoveGroup(group);
        }

        public event EventHandler CanExecuteChanged;
    }
}
