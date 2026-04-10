public abstract class SubTensionInterpreterBase
{
    protected CompositeSubscription subscriptions = new CompositeSubscription();
    protected readonly GameEventPublisher publisher = new GameEventPublisher();
    private bool isEnabled;

    protected SubTensionInterpreterBase(UnityEngine.Object source)
    {
        publisher.SetSource(source);
    }

    public void Enable()
    {
        if (isEnabled == true)
        {
            return;
        }

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        subscriptions = new CompositeSubscription();
        Subscribe(hub);
        isEnabled = true;
        OnEnabled();
    }

    public void Disable()
    {
        if (isEnabled == false)
        {
            return;
        }

        subscriptions.Dispose();
        subscriptions = new CompositeSubscription();
        isEnabled = false;
        OnDisabled();
        Reset();
    }

    public virtual void Tick(float deltaTime)
    {
    }

    protected abstract void Subscribe(GameEventHub hub);

    protected virtual void OnEnabled()
    {
    }

    protected virtual void OnDisabled()
    {
    }

    protected virtual void Reset()
    {
    }
}
