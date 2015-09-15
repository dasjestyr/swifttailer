using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Fievel.Wpf.Models.Observable;
using Fievel.Wpf.Pages;

namespace Fievel.Wpf.Commands
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
                lbx = FindAncestor<ListBox>(lbi);
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

        private T FindAncestor<T>(DependencyObject dependencyObject)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindAncestor<T>(parent);
        }

        public event EventHandler CanExecuteChanged;
    }
}
