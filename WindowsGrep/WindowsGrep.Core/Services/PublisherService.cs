namespace WindowsGrep.Core;

public class PublisherService
{
    private class SubscriberList
    {
        public readonly List<object> Handlers = new();
        public readonly object Lock = new object();
    }

    #region Fields..
    private readonly ILogger _logger;
    private ConcurrentDictionary<PublisherMessage, SubscriberList> _subscribers = new();
    #endregion Fields..

    #region Constructors..
    public PublisherService() { }
    public PublisherService(ILogger logger)
    {
        _logger = logger;
    }
    #endregion Constructors..

    #region Methods..
    public void Subscribe<T>(PublisherMessage messageType, Action<T> handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var subscriberList = _subscribers.GetOrAdd(messageType, _ => new SubscriberList());

        lock (subscriberList.Lock)
            subscriberList.Handlers.Add(handler);
    }

    public void Unsubscribe<T>(PublisherMessage messageType, Action<T> handler)
    {
        if (_subscribers.TryGetValue(messageType, out var subscriberList))
        {
            lock (subscriberList.Lock)
                subscriberList.Handlers.Remove(handler);
        }
    }

    public void Publish<T>(PublisherMessage messageType, params T[] messages)
    {
        if (_subscribers.TryGetValue(messageType, out var subscriberList))
        {
            // Create a snapshot to avoid issues if handlers are added/removed during invocation
            Action<T>[] handlersSnapshot;
            lock (subscriberList.Lock)
            {
                handlersSnapshot = new Action<T>[subscriberList.Handlers.Count];
                for (int i = 0; i < subscriberList.Handlers.Count; i++)
                    handlersSnapshot[i] = (Action<T>)subscriberList.Handlers[i];
            }

            foreach (var handler in handlersSnapshot)
            {
                try
                {
                    foreach (var message in messages)
                    {
                        handler(message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error invoking handler for message type {messageType}: {ex.Message}");
                }
            }
        }
    }

    public virtual void RemoveAllSubscribers()
    {
        _subscribers = new();
    }
    #endregion Methods..
}
