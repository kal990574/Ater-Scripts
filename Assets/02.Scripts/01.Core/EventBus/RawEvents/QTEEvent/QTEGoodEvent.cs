public readonly struct QTEGoodEvent: IGameEvent
{
    public GameEventContext Context { get; }
    
    public QTEGoodEvent(GameEventContext context)
    {
        Context = context;
    }
}
