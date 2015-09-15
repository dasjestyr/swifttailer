using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Fievel.Wpf.Models.Observable;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += DisplayException;
        }

        private void DisplayException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var item = e.Source as TabItem;
            if (item == null) return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, item, DragDropEffects.All);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {

            var itemTarget = e.Source as TabItem;
            var itemSource = e.Data.GetData(typeof (TabItem)) as TabItem;

            var pageContext = DataContext as MainViewModel;
            var targetContext = itemTarget?.DataContext as TailFile;
            var sourceContext = itemSource?.DataContext as TailFile;

            if (targetContext == null || sourceContext == null || pageContext == null) return;

            if (sourceContext.Order < targetContext.Order)
            {
                var sourceOrder = sourceContext.Order;
                sourceContext.Order = targetContext.Order;
                for (var i = targetContext.Order; i > sourceOrder; i--)
                {
                    var item = SessionTabs.Items[i] as TailFile;
                    if (item == null) continue;
                    item.Order--;
                }
                pageContext.BindGroups();
                pageContext.SaveOrder();
            }
            else
            {
                var sourceOrder = sourceContext.Order;
                sourceContext.Order = targetContext.Order;
                for (var i = targetContext.Order; i < sourceOrder; i++)
                {
                    var item = SessionTabs.Items[i] as TailFile;
                    if(item == null) continue;
                    item.Order++;
                }
                pageContext.BindGroups();
                pageContext.SaveOrder();
            }
        }

        private void LogFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
