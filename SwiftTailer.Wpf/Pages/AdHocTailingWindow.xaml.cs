using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for AdHocTailingWindow.xaml
    /// </summary>
    public partial class AdHocTailingWindow : Window
    {
        private AdHocTailingViewModel ViewModel => DataContext as AdHocTailingViewModel;

        public AdHocTailingWindow(string filePath)
        {
            InitializeComponent();

            ViewModel.WindowTitle = $"Tailing {Path.GetFileName(filePath)}";
            ViewModel.SetTail(filePath);
        }

        private void LogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The selection gets changed throught it's SelectedItem binding
            // This old school sauce is to deal with the fact that custom 
            // behaviors just weren't having me 

            var lbx = sender as ListBox;
            if (lbx == null) return;
            
            var tail = ViewModel.TailFile;

            if (tail == null) return;

            if (tail.FollowTail)
                lbx.ScrollIntoView(lbx.SelectedItem);
        }

        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbi = sender as ListBoxItem;
            if (lbi == null) return;

            StaticCommands.OpenLogLineCommand.Execute(lbi);
        }
    }
}
