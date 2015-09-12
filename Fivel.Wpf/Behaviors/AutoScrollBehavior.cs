using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Fivel.Wpf.Behaviors
{
    public class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        private double _height = 0.0d;
        private ScrollViewer _scrollViewer = null;

        protected override void OnAttached()
        {
            base.OnAttached();

            this._scrollViewer = base.AssociatedObject;
            this._scrollViewer.LayoutUpdated += new EventHandler(_scrollViewer_LayoutUpdated);
        }

        private void _scrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            if (Math.Abs(this._scrollViewer.ExtentHeight - _height) > 1)
            {
                this._scrollViewer.ScrollToVerticalOffset(this._scrollViewer.ExtentHeight);
                this._height = this._scrollViewer.ExtentHeight;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this._scrollViewer != null)
            {
                this._scrollViewer.LayoutUpdated -= new EventHandler(_scrollViewer_LayoutUpdated);
            }
        }
    }
}