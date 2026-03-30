public readonly struct QTEFailEvent
{
    public GameEventContext Context { get; }
    
    public QTEFailEvent(GameEventContext context)
    {
        Context = context;
    }
}