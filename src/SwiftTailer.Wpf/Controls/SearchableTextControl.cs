using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SwiftTailer.Wpf.Controls
{
    public class SearchableTextControl : Control
    {
        static SearchableTextControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableTextControl),
                new FrameworkPropertyMetadata(typeof(SearchableTextControl)));
        }

        #region DependencyProperties

        /// <summary>
        /// Text sandbox which is used to get or set the value from a dependency property.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        // Real implementation about TextProperty which  registers a dependency property with 
        // the specified property name, property type, owner type, and property metadata. 
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SearchableTextControl),
            new UIPropertyMetadata(string.Empty,
              UpdateControlCallBack));

        /// <summary>
        /// HighlightBackground sandbox which is used to get or set the value from a dependency property,
        /// if it gets a value,it should be forced to bind to a Brushes type.
        /// </summary>
        public Brush HighlightBackground
        {
            get { return (Brush)GetValue(HighlightBackgroundProperty); }
            set { SetValue(HighlightBackgroundProperty, value); }
        }


        // Real implementation about HighlightBackgroundProperty which registers a dependency property 
        // with the specified property name, property type, owner type, and property metadata. 
        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(SearchableTextControl),
            new UIPropertyMetadata(Brushes.Yellow, UpdateControlCallBack));

        /// <summary>
        /// HighlightForeground sandbox which is used to get or set the value from a dependency property,
        /// if it gets a value,it should be forced to bind to a Brushes type.
        /// </summary>
        public Brush HighlightForeground
        {
            get { return (Brush)GetValue(HighlightForegroundProperty); }
            set { SetValue(HighlightForegroundProperty, value); }
        }


        // Real implementation about HighlightForegroundProperty which registers a dependency property with
        // the specified property name, property type, owner type, and property metadata. 
        public static readonly DependencyProperty HighlightForegroundProperty =
            DependencyProperty.Register("HighlightForeground", typeof(Brush), typeof(SearchableTextControl),
            new UIPropertyMetadata(Brushes.Black, UpdateControlCallBack));

        /// <summary>
        /// IsMatchCase sandbox which is used to get or set the value from a dependency property,
        /// if it gets a value,it should be forced to bind to a bool type.
        /// </summary>
        public bool IsMatchCase
        {
            get { return (bool)GetValue(IsMatchCaseProperty); }
            set { SetValue(IsMatchCaseProperty, value); }
        }

        // Real implementation about IsMatchCaseProperty which  registers a dependency property with
        // the specified property name, property type, owner type, and property metadata. 
        public static readonly DependencyProperty IsMatchCaseProperty =
            DependencyProperty.Register("IsMatchCase", typeof(bool), typeof(SearchableTextControl),
            new UIPropertyMetadata(true, UpdateControlCallBack));

        /// <summary>
        /// IsHighlight sandbox which is used to get or set the value from a dependency property,
        /// if it gets a value,it should be forced to bind to a bool type.
        /// </summary>
        public bool IsHighlight
        {
            get { return (bool)GetValue(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }

        // Real implementation about IsHighlightProperty which  registers a dependency property with
        // the specified property name, property type, owner type, and property metadata. 
        public static readonly DependencyProperty IsHighlightProperty =
            DependencyProperty.Register("IsHighlight", typeof(bool), typeof(SearchableTextControl),
            new UIPropertyMetadata(false, UpdateControlCallBack));

        /// <summary>
        /// SearchText sandbox which is used to get or set the value from a dependency property,
        /// if it gets a value,it should be forced to bind to a string type.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        /// <summary>
        /// Real implementation about SearchTextProperty which registers a dependency property with
        /// the specified property name, property type, owner type, and property metadata. 
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchableTextControl),
            new UIPropertyMetadata(string.Empty, UpdateControlCallBack));

        /// <summary>
        /// Create a call back function which is used to invalidate the rendering of the element, 
        /// and force a complete new layout pass.
        /// One such advanced scenario is if you are creating a PropertyChangedCallback for a 
        /// dependency property that is not  on a Freezable or FrameworkElement derived class that 
        /// still influences the layout when it changes.
        /// </summary>
        private static void UpdateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchableTextControl obj = d as SearchableTextControl;
            obj.InvalidateVisual();
        }
        #endregion

        /// <summary>
        /// override the OnRender method which is used to search for the keyword and highlight
        /// it when the operation gets the result.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            TextBlock displayTextBlock = this.Template.FindName("PART_TEXT", this) as TextBlock;

            if (string.IsNullOrEmpty(this.Text))
            {
                base.OnRender(drawingContext);
                return;
            }

            if (!this.IsHighlight)
            {
                displayTextBlock.Text = this.Text;
                base.OnRender(drawingContext);
                return;
            }

            displayTextBlock.Inlines.Clear();

            if(string.IsNullOrEmpty(SearchText))
                SearchText = string.Empty;
            
            var searchPhrases = this.IsMatchCase 
                ? SearchText.Trim().Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries) 
                : SearchText.Trim().ToUpper().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var compareText = this.IsMatchCase ? this.Text : this.Text.ToUpper();
            var displayText = this.Text;

            var matches = GetMatchIndices(searchPhrases, compareText);

            Run run;
            var position = 0;

            if (!matches.Any())
            {
                var runText = GenerateRun(displayText, false);
                displayTextBlock.Inlines.Add(runText);
                base.OnRender(drawingContext);
                return;
            }

            var firstMatchIndex = matches[0].Index;

            // if first match is not at start, grab first chunk of text
            if (firstMatchIndex != 0)
            {
                var firstChunk = displayText.Substring(0, firstMatchIndex);
                run = GenerateRun(firstChunk, false); // unformatted text
                displayTextBlock.Inlines.Add(run);
                position = firstMatchIndex;
            }

            // start building
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var nextIndex = matches.Count > i + 1
                    ? matches[i + 1].Index
                    : -1;

                // grab the match
                var runText = GetSubStringAndAdvancePosition(displayText, ref position, match.Length);
                run = GenerateRun(runText, true);

                if (run != null)
                    displayTextBlock.Inlines.Add(run);

                // if the next position is not a match, grab unformatted text up to the next match
                if (nextIndex != match.Index + 1 && nextIndex != -1)
                {
                    var length = nextIndex - position;
                    runText = GetSubStringAndAdvancePosition(displayText, ref position, length);
                    run = GenerateRun(runText, false);

                    if(run != null)
                        displayTextBlock.Inlines.Add(run);
                }
            }

            // get everything else
            if (position < displayText.Length)
            {
                var runText = displayText.Substring(position, displayText.Length - position);
                run = GenerateRun(runText, false);
                displayTextBlock.Inlines.Add(run);
            }

            base.OnRender(drawingContext);
        }

        private string GetSubStringAndAdvancePosition(string source, ref int position, int length)
        {
            var result = source.Substring(position, length);
            position += length;
            return result;
        }

        private static List<PhraseMatchInfo> GetMatchIndices(IEnumerable<string> searchPhrases, string searchTarget)
        {
            var matchInfo = new List<PhraseMatchInfo>();
            foreach (var phrase in searchPhrases)
            {
                if (searchTarget.IndexOf(phrase.Trim(), StringComparison.Ordinal) == -1)
                    continue;

                matchInfo.Add(new PhraseMatchInfo(
                    searchTarget.IndexOf(phrase.Trim(), StringComparison.Ordinal), 
                    phrase.Length));
            }

            return matchInfo
                .OrderBy(m => m.Index)
                .ToList();
        } 

        /// <summary>
        /// Set inline-level flow content element intended to contain a run of formatted or unformatted 
        /// text into your background and foreground setting.
        /// </summary>
        private Run GenerateRun(string runSegment, bool isHighlight)
        {
            if (string.IsNullOrEmpty(runSegment)) return null;

            var run = new Run(runSegment)
            {
                Background = isHighlight ? this.HighlightBackground : this.Background,
                Foreground = isHighlight ? this.HighlightForeground : this.Foreground,

                // Set the source text with the style which is Italic.
                //   FontStyle = isHighlight ? FontStyles.Italic : FontStyles.Normal,

                // Set the source text with the style which is Bold.
                FontWeight = isHighlight ? FontWeights.Bold : FontWeights.Normal
            };

            return run;
        }

        private class PhraseMatchInfo
        {
            public int Index { get; }

            public int Length { get; set; }

            public PhraseMatchInfo(int index, int length)
            {
                Index = index;
                Length = length;
            }
        }
    }
}
