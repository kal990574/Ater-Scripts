public readonly struct PuzzleFailEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public string GameObjectName { get; }

    public PuzzleFailEvent(GameEventContext context, int puzzleId , string gameObjectName)
    {
        Context = context;
        PuzzleId = puzzleId;
        GameObjectName = gameObjectName;
    }
}