using System;
using System.Windows.Input;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Commands
{
    /// <summary>
    /// To be used with the main tab view
    /// </summary>
    public class RemoveLogFromGroupCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var file = parameter as TailFile;
            file?.DeleteSelf();
        }

        public event EventHandler CanExecuteChanged;
    }
}
