using System;
using System.Windows.Controls;
using System.Windows.Input;
using Fievel.Wpf.Data;
using Fievel.Wpf.Models.Observable;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
{
    /// <summary>
    /// To be used with the main tab view
    /// </summary>
    public class RemoveLogFromGroupCommand : ICommand
    {
        private readonly TailFile _vm;

        public RemoveLogFromGroupCommand(TailFile vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var item = parameter as MenuItem;
            var file = item?.DataContext as TailFile;
            file?.DeleteSelf();
        }

        public event EventHandler CanExecuteChanged;
    }
}
