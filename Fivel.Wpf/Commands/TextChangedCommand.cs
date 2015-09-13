using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Fivel.Wpf.Models.Observable;

namespace Fivel.Wpf.Commands
{
    public class TextChangedCommand : ICommand
    {
        private readonly TailFile _vm;

        public TextChangedCommand(TailFile vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await ClearHighlights();
            await SetHighlights(_vm.SearchPhrase);
         
            Trace.WriteLine($"Found {_vm.LogLines.Count(line => line.Highlight)} results!");
        }

        private async Task ClearHighlights()
        {
            await Task.Run(() =>
            {
                foreach (var item in _vm.LogLines)
                {
                    item.Highlight = false;
                }
            });
        }

        private async Task SetHighlights(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return;

            await Task.Run(() =>
            {
                foreach (var line in _vm.LogLines
                    .Where(line => line.Content.Contains(phrase)))
                {
                    line.Highlight = true;
                }
            });
        }

        public event EventHandler CanExecuteChanged;
    }
}