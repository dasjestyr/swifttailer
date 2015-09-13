using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fivel.Wpf.Models.Observable;
using Fivel.Wpf.ViewModels;

namespace Fivel.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
