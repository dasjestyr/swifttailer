using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Fievel.Wpf.Models.Observable;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
{
    /// <summary>
    /// To be used with the main tab view
    /// </summary>
    public class RemoveLogFromGroupCommand : ICommand
    {
        private readonly MainViewModel _vm;

        public RemoveLogFromGroupCommand(MainViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var tail = parameter as TailFile;
        }

        public event EventHandler CanExecuteChanged;
    }
}
