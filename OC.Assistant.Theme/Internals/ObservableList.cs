using System.Collections.Specialized;

namespace OC.Assistant.Theme.Internals;

/// <inheritdoc cref="List{T}" />
internal class ObservableList<T> : List<T>, INotifyCollectionChanged
{
    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Triggers the <see cref="CollectionChanged"/> event,
    /// indicating that the entire collection has changed.
    /// This causes any observers to refresh their view of the data.
    /// </summary>
    public void Notify()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}