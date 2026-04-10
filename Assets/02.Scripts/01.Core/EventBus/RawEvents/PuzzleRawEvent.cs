public readonly struct PuzzleResultRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public EPuzzleType PuzzleType { get; }
    public EPuzzleResult Result { get; }

    public PuzzleResultRawEvent(
        GameEventContext context,
        EPuzzleType puzzleType,
        EPuzzleResult result)
    {
        Context = context;
        PuzzleType = puzzleType;
        Result = result;
    }
}