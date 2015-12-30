using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SwiftTailer.Wpf.Filters;
using SwiftTailer.Wpf.Models;

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

        ///// <summary>
        ///// HighlightBackground sandbox which is used to get or set the value from a dependency property,
        ///// if it gets a value,it should be forced to bind to a Brushes type.
        ///// </summary>
        //public Brush HighlightBackground
        //{
        //    get { return (Brush)GetValue(HighlightBackgroundProperty); }
        //    set { SetValue(HighlightBackgroundProperty, value); }
        //}


        //// Real implementation about HighlightBackgroundProperty which registers a dependency property 
        //// with the specified property name, property type, owner type, and property metadata. 
        //public static readonly DependencyProperty HighlightBackgroundProperty =
        //    DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(SearchableTextControl),
        //    new UIPropertyMetadata(Brushes.Yellow, UpdateControlCallBack));

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
        /// Gets or sets the error text.
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string ErrorText
        {
            get { return (string) GetValue(ErrorTextProperty); }
            set { SetValue(ErrorTextProperty, value);}
        }

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register("ErrorText", typeof(string), typeof(SearchableTextControl),
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
            var displayTextBlock = Template.FindName("PART_TEXT", this) as TextBlock;
            if(displayTextBlock == null)
                throw new InvalidOperationException("Cannot virtualize log lines without PART_TEXT");

            if (string.IsNullOrEmpty(Text))
            {
                base.OnRender(drawingContext);
                return;
            }

            if (!IsHighlight)
            {
                displayTextBlock.Text = Text;
                base.OnRender(drawingContext);
                return;
            }

            displayTextBlock.Inlines.Clear();

            if(string.IsNullOrEmpty(SearchText))
                SearchText = string.Empty;

            /* BEGIN SEARCH MAGIC */

            var displayText = Text;
            var matches = GetMatchInfos();

            // no matches, apply no filter
            if (!matches.Any())
            {
                var runText = GenerateRun(displayText, false, LogHighlight.None);
                displayTextBlock.Inlines.Add(runText);
                base.OnRender(drawingContext);
                return;
            }

            // start building
            BuildTextLine(matches, displayTextBlock);

            // render
            base.OnRender(drawingContext);
        }

        private List<PhraseMatchInfo> GetMatchInfos()
        {
            var searchPhrases = IsMatchCase
                ? SearchText.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                : SearchText.Trim().ToUpper().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var errorPhrases = IsMatchCase
                ? ErrorText.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                : ErrorText.Trim().ToUpper().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var compareText = IsMatchCase ? Text : Text.ToUpper();

            var findMatches = GetMatchIndices(searchPhrases, compareText, LogHighlight.Find);
            findMatches.AddRange(GetMatchIndices(errorPhrases, compareText, LogHighlight.Error));
            var matches = findMatches.OrderBy(m => m.Index).ToList();

            return matches;
        }

        private void BuildTextLine(IReadOnlyList<PhraseMatchInfo> matches, TextBlock displayTextBlock)
        {
            Run run;
            var position = 0;
            var firstMatchIndex = matches[0].Index;
            var displayText = Text;

            // if first match is not at start, grab first chunk of text
            if (firstMatchIndex != 0)
            {
                var firstChunk = displayText.Substring(0, firstMatchIndex);
                run = GenerateRun(firstChunk, false, LogHighlight.None); // unformatted text
                displayTextBlock.Inlines.Add(run);
                position = firstMatchIndex;
            }

            // build up inner text
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var nextIndex = matches.Count > i + 1
                    ? matches[i + 1].Index
                    : -1;

                // grab the match
                var runText = SubstringAndAdvance(displayText, ref position, match.Length);
                run = GenerateRun(runText, true, match.Highlight);

                if (run != null)
                    displayTextBlock.Inlines.Add(run);

                // if the next position is not a match, grab unformatted text up to the next match
                if (nextIndex != match.Index + 1 && nextIndex != -1)
                {
                    var length = nextIndex - position;
                    runText = SubstringAndAdvance(displayText, ref position, length);
                    run = GenerateRun(runText, false, LogHighlight.None);

                    if (run != null)
                        displayTextBlock.Inlines.Add(run);
                }
            }

            // get everything else
            if (position < displayText.Length)
            {
                var runText = displayText.Substring(position, displayText.Length - position);
                run = GenerateRun(runText, false, LogHighlight.None);
                displayTextBlock.Inlines.Add(run);
            }
        }

        private static string SubstringAndAdvance(string source, ref int position, int length)
        {
            if (length < 0) return source;

            var result = source.Substring(position, length);
            position += length;
            return result;
        }

        private static List<PhraseMatchInfo> GetMatchIndices(IEnumerable<string> searchPhrases, string searchTarget, LogHighlight highlight)
        {
            // DEFECT: this only marks the first instance of the match
            // DEFECT: this does not prevent collisions (same search term in multiple categories). Create a priority.

            // Don't LINQify until defect are fixed
            var matchInfo = new List<PhraseMatchInfo>();
            foreach (var phrase in searchPhrases)
            {
                if (searchTarget.IndexOf(phrase.Trim(), StringComparison.Ordinal) == -1)
                    continue;

                matchInfo.Add(new PhraseMatchInfo(
                    searchTarget,
                    searchTarget.IndexOf(phrase.Trim(), StringComparison.Ordinal), 
                    phrase.Length,
                    highlight));
            }

            return matchInfo
                .OrderBy(m => m.Index)
                .ToList();
        } 

        private Run GenerateRun(string runSegment, bool isHighlight, LogHighlight highlight)
        {
            if (string.IsNullOrEmpty(runSegment)) return null;

            var run = new Run(runSegment)
            {
                // NOTE: This ignores what is set in the view/xaml
                Background = isHighlight ? new SolidColorBrush(highlight.HighlightColor) : this.Background,
                Foreground = isHighlight ? this.HighlightForeground : this.Foreground,

                // ** Leave this here as reference for pinned lines **/

                // Set the source text with the style which is Italic.
                //   FontStyle = isHighlight ? FontStyles.Italic : FontStyles.Normal,

                // Set the source text with the style which is Bold.
                FontWeight = isHighlight ? FontWeights.Bold : FontWeights.Normal
            };

            return run;
        }
    }
}
