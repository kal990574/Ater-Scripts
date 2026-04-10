using UnityEngine;

public enum EInteractObjectEventType
{
    Default = 0,
    DoorOpen = 1,
    Unlock = 2
}

public readonly struct ItemAcquiredRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public int ItemId { get; }

    public ItemAcquiredRawEvent(GameEventContext context, int itemId)
    {
        Context = context;
        ItemId = itemId;
    }
}

public readonly struct ObjectInteractedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public EInteractObjectEventType Type { get; }

    public ObjectInteractedRawEvent(GameEventContext context, EInteractObjectEventType type)
    {
        Context = context;
        Type = type;
    }
}

public readonly struct InteractPromptRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public bool IsVisible { get; }
    public string PromptText { get; }

    public InteractPromptRawEvent(GameEventContext context, bool isVisible, string promptText)
    {
        Context = context;
        IsVisible = isVisible;
        PromptText = promptText;
    }
}
