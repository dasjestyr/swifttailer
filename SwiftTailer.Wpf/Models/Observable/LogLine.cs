using System.Collections.ObjectModel;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class LogLine : ModelBase
    {
        private LogHighlight _highlight;
        private string _lineContext;
        private bool _pinned;

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
        /// Gets or sets the line context.
        /// </summary>
        /// <value>
        /// The line context.
        /// </value>
        public string LineContext
        {
            get { return string.IsNullOrEmpty(_lineContext) ? Content : _lineContext; }
            set
            {
                _lineContext = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LogLine"/> is pinning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if pinning; otherwise, <c>false</c>.
        /// </value>
        public bool Pinned
        {
            get { return _pinned; }
            set
            {
                _pinned = value;
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
            LineContext = string.Empty;
            Highlight = LogHighlight.None;
        }
    }
}