public readonly struct QTEGoodEvent: IGameEvent
{
    public GameEventContext Context { get; }
    
    public string GameObjectName { get; }
    
    public QTEGoodEvent(GameEventContext context, string gameObjectName)
    {
        Context = context;
        GameObjectName = gameObjectName;
    }
}