using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Models.Observable;
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
            var lbi = parameter as UIElement;

            if (lbx == null)
            {
                lbx = lbi.FindAncestor<ListBox>();
                if (lbx == null) return;
            }

            var lines = lbx.SelectedItems.Cast<LogLine>().ToList();
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line.Content);
            }

            var viewer = new ViewSelectionWindow(sb.ToString());
            viewer.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}
