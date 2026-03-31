public readonly struct PuzzleFailEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public PuzzleFailEvent(GameEventContext context, int puzzleId)
    {
        Context = context;
        PuzzleId = puzzleId;
    }
}
