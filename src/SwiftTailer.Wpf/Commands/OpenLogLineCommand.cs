using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Models;
using SwiftTailer.Wpf.Pages;
using SwiftTailer.Wpf.Utility;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenLogLineCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var lbx = parameter as ListBox;
            var lbi = parameter as ListBoxItem;

            if (lbx == null)
            {
                lbx = lbi.FindAncestor<ListBox>();
                if (lbx == null) return;
            }

            var lines = lbx.SelectedItems.Cast<LogLine>().ToList();

            var viewer = new ViewSelectionWindow(lines);
            viewer.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}
