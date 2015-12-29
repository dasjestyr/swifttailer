using System.Linq;
using System.Windows;
using Dragablz;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Infrastructure
{
    public class IntertabClient : IInterTabClient
    {
        private readonly ITailerWindowFactory _tailerWindowFactory;

        public IntertabClient(ITailerWindowFactory tailerWindowFactory)
        {
            _tailerWindowFactory = tailerWindowFactory;
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var tailer = _tailerWindowFactory.Create();
            return new NewTabHost<Window>(tailer, tailer.SessionTabControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return Application.Current.Windows.OfType<MainWindow>().Count() == 1
                ? TabEmptiedResponse.DoNothing
                : TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
