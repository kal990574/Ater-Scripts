public readonly struct PuzzleSuccessEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public string GameObjectName { get; }

    public PuzzleSuccessEvent(GameEventContext context, int puzzleId , string gameObjectName)
    {
        Context = context;
        PuzzleId = puzzleId;
        GameObjectName = gameObjectName;
    }
}