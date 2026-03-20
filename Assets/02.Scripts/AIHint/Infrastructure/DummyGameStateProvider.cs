using _02.Scripts.AIHint.Domain;
using System.Collections.Generic;

namespace _02.Scripts.AIHint.Infrastructure
{
    public class DummyGameStateProvider : IGameStateProvider
    {
        public int CurrentChapter => 1;

        public IReadOnlyList<string> GetInventory()
        {
            return new List<string> { "item_old_key" };
        }

        public IReadOnlyList<string> GetSolvedPuzzles()
        {
            return new List<string> { "puzzle_drawer" };
        }
    }
}