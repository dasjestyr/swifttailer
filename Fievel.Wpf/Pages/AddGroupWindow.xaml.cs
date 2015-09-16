using System.Windows;

namespace Fievel.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for EditGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        public AddGroupWindow()
        {
            InitializeComponent();
            NameTextBox.Focus();
        }
    }
}
