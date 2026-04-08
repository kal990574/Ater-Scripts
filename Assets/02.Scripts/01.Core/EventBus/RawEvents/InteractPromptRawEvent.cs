using UnityEngine;

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
