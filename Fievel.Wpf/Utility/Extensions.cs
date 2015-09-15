using System.Windows;
using System.Windows.Media;

namespace Fievel.Wpf.Utility
{
    public static class Extensions
    {
        public static T FindAncestor<T>(this DependencyObject obj) 
            where T : DependencyObject
        {
            // done as a loop to avoid explicit recursion
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                if (parent == null) return null;

                var parentT = parent as T;
                if (parentT != null) return parentT;
                obj = parent;
            }
        }
    }
}
