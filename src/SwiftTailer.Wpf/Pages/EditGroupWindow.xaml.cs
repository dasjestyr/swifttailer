using System.Windows;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for AddGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window
    {
        public EditGroupWindow()
        {
            InitializeComponent();
            NameTextBox.Focus();
        }

        public EditGroupWindow(LogGroup group)
            : this()
        {
            var vm = DataContext as AddGroupViewModel;
            if (vm == null) return;
            vm.Group = group;
        }
    }
}
