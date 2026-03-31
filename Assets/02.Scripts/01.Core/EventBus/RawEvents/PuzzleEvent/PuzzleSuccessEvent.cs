public readonly struct PuzzleSuccessEvent: IGameEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public PuzzleSuccessEvent(GameEventContext context, int puzzleId)
    {
        Context = context;
        PuzzleId = puzzleId;
    }
}
