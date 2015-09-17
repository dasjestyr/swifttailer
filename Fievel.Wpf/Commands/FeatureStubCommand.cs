using System;
using System.Windows;
using System.Windows.Input;

namespace Fievel.Wpf.Commands
{
    public class FeatureStubCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var message = parameter as string;
            MessageBox.Show(message, "Feature Stub", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
