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
            await SetHighlights(_vm.SearchPhrase);
         
            Trace.WriteLine($"Found {_vm.LogLines.Count(line => line.Highlight.Category == HighlightCategory.Find)} results!");
        }

        private async Task SetHighlights(string phrase)
        {
            // this should clear the highlight before applying new ones

            // optional filters should be captured and dynamically
            // added to this list. How do we determine precedence?
            await Task.Run(() => HighlightApplicator.Apply(
                _vm.LogLines,
                new ClearHighlitersFilter(),
                new SearchHighlightFilter(phrase)));
        }

        public event EventHandler CanExecuteChanged;
    }
}