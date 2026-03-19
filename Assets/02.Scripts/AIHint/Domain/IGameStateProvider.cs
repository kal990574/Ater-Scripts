using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain
{
    public interface IGameStateProvider
    {
        int CurrentChapter { get; }
        string CurrentRoom { get; }
        IReadOnlyList<string> GetInventory();
        IReadOnlyList<string> GetSolvedPuzzles();
    }
}