using System;

namespace SwiftTailer.Wpf.Infrastructure
{
    public interface IProperty<out T> : IDisposable
    {
        T Value { get; }
    }
}
