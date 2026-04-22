using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain.Models
{
    public sealed class PlayerHintState
    {
        public int CurrentChapter { get; }
        public IReadOnlyList<string> Inventory { get; }
        public IReadOnlyList<string> CompletedTasks { get; }

        public PlayerHintState(
            int currentChapter,
            IReadOnlyList<string> inventory,
            IReadOnlyList<string> completedTasks)
        {
            CurrentChapter = currentChapter;
            Inventory = inventory;
            CompletedTasks = completedTasks;
        }
    }
}