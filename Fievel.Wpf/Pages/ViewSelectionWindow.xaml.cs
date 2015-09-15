using System.Windows;
using Fievel.Wpf.ViewModels;

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

            var vm = new ViewSelectionViewModel();
            DataContext = vm;

            vm.Content = content;
        }
    }
}
