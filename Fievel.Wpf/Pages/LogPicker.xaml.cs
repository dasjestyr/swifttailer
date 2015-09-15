using System;
using System.Windows;
using Fievel.Wpf.Data;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Pages
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
