using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SwiftTailer.Wpf.Filters;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Commands
{
    public class ApplyUserInputHighlightCommand : ICommand
    {
        private readonly TailFile _vm;

        public ApplyUserInputHighlightCommand(TailFile vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            // TODO: maybe clear and apply at the same time?
            // Right now, this isn't a performance hit (2000 lines in 3ms)
            await ClearHighlights();
            await SetHighlights(_vm.SearchPhrase);
         
            Trace.WriteLine($"Found {_vm.LogLines.Count(line => line.Highlight.Category == LogHighlight.HighlightCategory.Find)} results!");
        }

        private async Task ClearHighlights()
        {
            await Task.Run(() =>
            {
                foreach (var item in _vm.LogLines)
                {
                    item.Highlight = LogHighlight.None;
                }
            });
        }

        private async Task SetHighlights(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return;

            // this might be a bit contrived for one filter, 
            // but it is just to show the future usage of the filter chain
             var chain = new LogLineFilterChain(new SearchHilightFilter(phrase));

            await Task.Run(() => chain.ApplyFilterChain(_vm.LogLines, false));
        }

        public event EventHandler CanExecuteChanged;
    }
}