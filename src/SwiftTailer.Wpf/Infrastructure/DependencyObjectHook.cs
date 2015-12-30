using System.Windows;

namespace SwiftTailer.Wpf.Infrastructure
{
    public interface IDependencyObjectReceiver
    {
        void Receive(DependencyObject value);
    }

    /// <summary>
    /// Attached property class allows something to run when a "Receiver" is set.
    /// </summary>
    public static class DependencyObjectHook
    {
        public static readonly DependencyProperty ReceiverProperty =
            DependencyProperty.RegisterAttached(
                "Receiver", 
                typeof(IDependencyObjectReceiver), 
                typeof(DependencyObjectHook), 
                new PropertyMetadata(default(IDependencyObjectReceiver), PropertyChanged));

        public static void SetReceiver(UIElement element, IDependencyObjectReceiver value)
        {
            element.SetValue(ReceiverProperty, value);
        }

        public static IDependencyObjectReceiver GetReceiver(UIElement element)
        {
            return (IDependencyObjectReceiver)element.GetValue(ReceiverProperty);
        }

        /// <summary>
        /// Executes when "Receiver" is set. If args.NewVlue is of type <see cref="IDependencyObjectReceiver"/>, then Receive() will be called with the host object as its argument.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        public static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var receiver = args.NewValue as IDependencyObjectReceiver;
            receiver?.Receive(sender);
        }
    }
}
