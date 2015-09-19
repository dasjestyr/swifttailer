using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += DisplayException;
        }

        private void DisplayException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Global error catch 
            // Doesn't really allow the app to resume, but it's better than logs
            // for now
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "error.log");
            var logPath = Path.Combine(Settings.WorkingDirectory, "error.log");

            using(var sw = File.AppendText(logPath))
            {
                sw.WriteLine($"{DateTime.Now.ToLongDateString()} :: {e.Exception.Message} :: {e.Exception}");
            }

            MessageBox.Show($"e.Exception.Message :: {e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The selection gets changed throught it's SelectedItem binding
            // This old school sauce is to deal with the fact that custom 
            // behaviors just weren't having me 

            var lbx = sender as ListBox;
            if (lbx == null) return;

            var vm = DataContext as MainViewModel;
            var tail = vm.SelectedTail;

            if (tail == null) return;

            if(tail.FollowTail)
                lbx.ScrollIntoView(lbx.SelectedItem);
        }

        //private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    var item = e.Source as TabItem;
        //    if (item == null) return;

        //    if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
        //    {
        //        DragDrop.DoDragDrop(item, item, DragDropEffects.All);
        //    }
        //}

        //private void TabItem_Drop(object sender, DragEventArgs e)
        //{
        //    var itemTarget = e.Source as TabItem;
        //    var itemSource = e.Data.GetData(typeof (TabItem)) as TabItem;

        //    var pageContext = DataContext as MainViewModel;
        //    var targetContext = itemTarget?.DataContext as TailFile;
        //    var sourceContext = itemSource?.DataContext as TailFile;

        //    if (targetContext == null || sourceContext == null || pageContext == null) return;

        //    if (sourceContext.Order < targetContext.Order)
        //    {
        //        var sourceOrder = sourceContext.Order;
        //        sourceContext.Order = targetContext.Order;
        //        for (var i = targetContext.Order; i > sourceOrder; i--)
        //        {
        //            var item = SessionTabs.Items[i] as TailFile;
        //            if (item == null) continue;
        //            item.Order--;
        //        }
        //        pageContext.BindGroups();
        //        pageContext.SaveOrder();
        //    }
        //    else
        //    {
        //        var sourceOrder = sourceContext.Order;
        //        sourceContext.Order = targetContext.Order;
        //        for (var i = targetContext.Order; i < sourceOrder; i++)
        //        {
        //            var item = SessionTabs.Items[i] as TailFile;
        //            if(item == null) continue;
        //            item.Order++;
        //        }
        //        pageContext.BindGroups();
        //        pageContext.SaveOrder();
        //    }
        //}
        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbi = sender as ListBoxItem;
            if (lbi == null) return;

            StaticCommands.OpenLogLineCommand.Execute(lbi);
        }
    }
}
