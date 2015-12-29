using System;
using System.Windows;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for LogPicker.xaml
    /// </summary>
    public partial class LogPicker : Window
    {
        public LogPicker()
        {
            InitializeComponent();
        }

        public LogPicker(LogGroup selectedGroup)
            : this()
        {
            var vm = DataContext as LogPickerDialogViewModel;
            if(vm == null)
                throw new InvalidOperationException("Group context not found!");

            vm.SelectedGroup = selectedGroup;
        }
    }
}
