using System.Windows.Controls;
using SwiftTailer.Wpf.Infrastructure.Messaging;

namespace SwiftTailer.Wpf.Controls.FileDrop
{
    /// <summary>
    /// Interaction logic for FileDropDialog.xaml
    /// </summary>
    public partial class FileDropDialog : UserControl
    {
        public FileDropDialog()
        {
            InitializeComponent();
            MessageBroker.Broadcast(new WindowFocusedRequestMessage());
        }
    }
}
