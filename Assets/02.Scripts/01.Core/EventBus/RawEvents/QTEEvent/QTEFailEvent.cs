public readonly struct QTEFailEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string GameObjectName { get; }
    
    public QTEFailEvent(GameEventContext context, string gameObjectName)
    {
        Context = context;
        GameObjectName = gameObjectName;
    }
}