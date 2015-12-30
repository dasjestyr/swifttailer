namespace SwiftTailer.Wpf.Infrastructure.Messaging
{
    public class WindowFocusedRequestMessage : DomainEvent
    {
        public string Reason { get; set; }

        public WindowFocusedRequestMessage()
        {
            
        }
        public WindowFocusedRequestMessage(string reason)
        {
            Reason = reason;
        }
    }
}
