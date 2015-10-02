using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    public class CompareWithClipboardCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var tb = parameter as TextBox;
            if (tb == null)
            {
                Trace.WriteLine("Did not get a textbox to work with.", ToString());
                return;
            }
            
            var selectedText = tb.SelectedText;
            var areEqual = !string.IsNullOrEmpty(selectedText) &&
                       Clipboard.GetText().Equals(selectedText, StringComparison.OrdinalIgnoreCase);

            MessageBox.Show(
                string.Format("Selected text is{0} equal to the text on your clipboard!",
                    areEqual ? string.Empty : " NOT"), 
                "Result", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
    }
}
