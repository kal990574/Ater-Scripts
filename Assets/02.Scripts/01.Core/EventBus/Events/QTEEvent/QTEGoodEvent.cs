public readonly struct QTEGoodEvent
{
    public GameEventContext Context { get; }
    
    public QTEGoodEvent(GameEventContext context)
    {
        Context = context;
    }
}