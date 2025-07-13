namespace WindowsGrep.Core;

public class PublisherService
{
    private class SubscriberList
    {
        public readonly List<object> Handlers = new();
        public readonly object Lock = new object();
    }

    #region Fields..
    private readonly ConcurrentDictionary<Type, SubscriberList> _subscribers = new();
    private readonly ILogger _logger;
    #endregion Fields..

    #region Constructors..
    public PublisherService() { }
    public PublisherService(ILogger logger)
    {
        _logger = logger;
    }
    #endregion Constructors..

    #region Methods..
    public void Subscribe<T>(Action<T> handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(T);
        var subscriberList = _subscribers.GetOrAdd(messageType, _ => new SubscriberList());

        lock (subscriberList.Lock)
            subscriberList.Handlers.Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(T);
        if (_subscribers.TryGetValue(messageType, out var subscriberList))
        {
            lock (subscriberList.Lock)
            {
                subscriberList.Handlers.Remove(handler);
                if (subscriberList.Handlers.Count == 0)
                    _subscribers.TryRemove(messageType, out _);
            }
        }
    }

    public void Publish<T>(T message)
    {
        var messageType = typeof(T);
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
                    handler(message);
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
        _subscribers.Clear();
    }
    #endregion Methods..
}
