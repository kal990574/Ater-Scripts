using System.Collections.Generic;

namespace _02.Scripts.Core.Domain
{
    public interface IGameStateProvider
    {
        int CurrentChapter { get; }
        IReadOnlyList<string> GetInventory();
        IReadOnlyList<string> GetCompletedTasks();
    }
}