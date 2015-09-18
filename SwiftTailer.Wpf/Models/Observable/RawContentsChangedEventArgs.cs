using System;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class RawContentsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the previous text.
        /// </summary>
        /// <value>
        /// The previous text.
        /// </value>
        public string PreviousText { get; set; }

        /// <summary>
        /// Gets or sets the new text.
        /// </summary>
        /// <value>
        /// The new text.
        /// </value>
        public string NewText { get; set; }

        /// <summary>
        /// Gets or sets the tail identifier.
        /// </summary>
        /// <value>
        /// The tail identifier.
        /// </value>
        public string TailId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawContentsChangedEventArgs" /> class.
        /// </summary>
        /// <param name="previousText">The previous text.</param>
        /// <param name="newText">The new text.</param>
        public RawContentsChangedEventArgs(string previousText, string newText, string tailId)
        {
            PreviousText = previousText;
            NewText = newText;
            TailId = tailId;
        }
    }
}