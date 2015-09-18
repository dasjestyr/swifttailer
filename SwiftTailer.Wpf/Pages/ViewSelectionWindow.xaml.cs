using System.Windows;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ViewSelectionWindow.xaml
    /// </summary>
    public partial class ViewSelectionWindow : Window
    {
        public ViewSelectionWindow(string content)
        {
            InitializeComponent();

            var vm = new ViewSelectionViewModel();
            DataContext = vm;

            vm.Content = content;
        }
    }
}
