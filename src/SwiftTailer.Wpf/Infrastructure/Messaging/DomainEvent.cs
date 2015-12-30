namespace SwiftTailer.Wpf.Infrastructure.Messaging
{
    public class DomainEvent
    {
        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        /// <value>
        /// The name of the event.
        /// </value>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        public DomainEvent()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DomainEvent(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="eventName">Name of the event.</param>
        public DomainEvent(string message, string eventName)
            : this(message)
        {
            EventName = eventName;
        }
    }
}