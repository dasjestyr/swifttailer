using System;
using System.Windows;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    public class CloseWindowCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = parameter as Window;
            if (w == null) return;

            w.Hide();
            w.Close();
        }

        public event EventHandler CanExecuteChanged;
    }
}
