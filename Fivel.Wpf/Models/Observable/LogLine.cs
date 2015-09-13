using Provausio.Common.Portable;

namespace Fivel.Wpf.Models.Observable
{
    public class LogLine : ModelBase
    {
        private bool _highlight;

        public string Content { get; set; }

        public bool Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                OnPropertyChanged();
            }
        }

        public LogLine(string content, bool highlight)
        {
            Content = content;
            Highlight = highlight;
        }
    }
}