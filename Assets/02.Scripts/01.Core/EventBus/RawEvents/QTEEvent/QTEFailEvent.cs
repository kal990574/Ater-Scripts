public readonly struct QTEFailEvent: IGameEvent
{
    public GameEventContext Context { get; }

    public QTEFailEvent(GameEventContext context)
    {
        Context = context;
    }
}
