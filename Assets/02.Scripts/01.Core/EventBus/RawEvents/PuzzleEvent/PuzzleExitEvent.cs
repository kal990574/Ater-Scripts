public readonly struct PuzzleExitEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public PuzzleExitEvent(GameEventContext context, int puzzleId)
    {
        Context = context;
        PuzzleId = puzzleId;
    }
}