public readonly struct PuzzleStartEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public PuzzleStartEvent(GameEventContext context, int puzzleId)
    {
        Context = context;
        PuzzleId = puzzleId;
    }
}
