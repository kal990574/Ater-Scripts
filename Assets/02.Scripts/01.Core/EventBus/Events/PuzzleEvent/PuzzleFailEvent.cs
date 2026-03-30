public readonly struct PuzzleFailEvent
{
    public GameEventContext Context { get; }
    public int PuzzleId { get; }
    public string PuzzleName { get; }

    public PuzzleFailEvent(GameEventContext context, int puzzleId , string puzzleName)
    {
        Context = context;
        PuzzleId = puzzleId;
        PuzzleName = puzzleName;
    }
}