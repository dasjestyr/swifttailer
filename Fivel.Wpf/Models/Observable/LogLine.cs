using Provausio.Common.Portable;

namespace Fivel.Wpf.Models.Observable
{
    public class LogLine : ModelBase
    {
        private LogHighlight _highlight;

        public string Content { get; set; }

        public LogHighlight Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                OnPropertyChanged();
            }
        }

        public LogLine(string content, LogHighlight highlight)
        {
            Content = content;
            Highlight = highlight;
        }
    }
}