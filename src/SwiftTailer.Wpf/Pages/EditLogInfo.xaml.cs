using System.Windows;
using SwiftTailer.Wpf.Models.Observable;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for EditLogInfo.xaml
    /// </summary>
    public partial class EditLogInfo : Window
    {
        public EditLogInfo(TailFile tail)
        {
            InitializeComponent();
            DataContext = new EditLogViewModel(tail);
        }
    }
}
