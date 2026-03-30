public readonly struct PuzzleSuccessEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public string PuzzleName { get; }

    public PuzzleSuccessEvent(GameEventContext context, int puzzleId , string puzzleName)
    {
        Context = context;
        PuzzleId = puzzleId;
        PuzzleName = puzzleName;
    }
}