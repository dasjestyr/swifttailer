using System.Windows;

namespace Fievel.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ViewSelectionWindow.xaml
    /// </summary>
    public partial class ViewSelectionWindow : Window
    {
        public ViewSelectionWindow(string content)
        {
            InitializeComponent();
            Content.Text = content;
        }
    }
}
