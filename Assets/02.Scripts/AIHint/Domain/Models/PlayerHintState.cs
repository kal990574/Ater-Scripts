using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class PlayerHintState
    {
        public int CurrentChapter { get; }
        public IReadOnlyList<string> Inventory { get; }
        public IReadOnlyList<string> SolvedPuzzles { get; }

        public PlayerHintState(
            int currentChapter,
            IReadOnlyList<string> inventory,
            IReadOnlyList<string> solvedPuzzles)
        {
            CurrentChapter = currentChapter;
            Inventory = inventory;
            SolvedPuzzles = solvedPuzzles;
        }
    }
}