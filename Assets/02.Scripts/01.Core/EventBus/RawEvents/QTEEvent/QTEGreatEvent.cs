public struct QTEGreatEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public string GameObjectName { get; }
    
    public QTEGreatEvent(GameEventContext context, string gameObjectName)
    {
        Context = context;
        GameObjectName = gameObjectName;
    }
}