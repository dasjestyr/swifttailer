using System.Collections.ObjectModel;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class LogLine : ModelBase
    {
        private LogHighlight _highlight;
        private ObservableCollection<LogLine> _context = new ObservableCollection<LogLine>();

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the highlight.
        /// </summary>
        /// <value>
        /// The highlight.
        /// </value>
        public LogHighlight Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets the context, if applicable.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public ObservableCollection<LogLine> Context
        {
            get { return _context; }
            set
            {
                _context = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLine" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="highlight">The highlight.</param>
        public LogLine(string content, LogHighlight highlight)
        {
            Content = content;
            Highlight = highlight;
        }

        /// <summary>
        /// Removes filter effects
        /// </summary>
        public void DeFilter()
        {
            Context.Clear();
            Highlight = LogHighlight.None;
        }
    }
}