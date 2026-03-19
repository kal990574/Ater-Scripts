using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class PlayerHintState
    {
        public int CurrentChapter { get; }
        public string CurrentRoom { get; }
        public IReadOnlyList<string> Inventory { get; }
        public IReadOnlyList<string> SolvedPuzzles { get; }
        
        public PlayerHintState(
            int currentChapter,
            string currentRoom,
            IReadOnlyList<string> inventory,
            IReadOnlyList<string> solvedPuzzles)
        {
            CurrentChapter = currentChapter;
            CurrentRoom = currentRoom;
            Inventory = inventory;
            SolvedPuzzles = solvedPuzzles;
        }
    }
}