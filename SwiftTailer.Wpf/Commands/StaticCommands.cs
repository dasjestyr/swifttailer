﻿using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Behaviors;

namespace SwiftTailer.Wpf.Commands
{
    /// <summary>
    /// Use as static commands or as a bucket for when gaining context to the view model is too difficult.
    /// </summary>
    public class StaticCommands
    {
        public static ICommand OpenLogLineCommand = new OpenLogLineCommand();
        
        public static ICommand CloseWindowCommand = new CloseWindowCommand();

        public static ICommand DeleteGroupCommand = new DeleteGroupCommand();

        public static ICommand EditGroupCommand = new EditGroupCommand();

        public static ICommand RemoveLogCommand = new RemoveLogFromGroupCommand();

        public static ICommand FeatureStubCommand = new FeatureStubCommand();

        public static ICommand ExportLogConfigCommand = new ExportLogConfigCommand();

        public static ICommand ImportLogConfigCommand = new ImportLogConfigCommand();

        public static ICommand OpenPathInExplorerCommand = new OpenPathInExplorerCommand();

        public static ICommand OpenLogConfigInExplorerCommand = new OpenLogConfigInExplorerCommand();

        public static ICommand ZipSelectedGroupCommand = new ZipGroupCommand();

        public static ICommand ZipGroupAndEmailCommand = new ZipGroupAndEmailCommand();

        public static ICommand OpenAdHocTailCommand = new OpenAdHocTailCommand();

        public static ICommand PinLogLineCommand = new PinLogLineCommand();

        public static ICommand UnpinLogLineCommand = new UnpinLogLineCommand();

        public static ICommand OpenSettingsCommand = new OpenSettingsCommand();

        public static ICommand CompareToClipboardCommand = new CompareWithClipboardCommand();

        public static ICommand PingSelectionCommand = new PingSelectionCommand();

        public static ICommand OpenEditLogWindowCommand = new OpenEditLogWindowCommand();
    }
}