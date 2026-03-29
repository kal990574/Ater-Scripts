using System;
using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain.Models
{
    [Serializable]
    public sealed class ChapterData
    {
        public int Chapter;
        public string Name;
        public List<PuzzleData> Puzzles;
        public List<ItemData> Items;
    }

    [Serializable]
    public sealed class PuzzleData
    {
        public string Id;
        public string Name;
        public string Description;
        public List<string> RequiredItems;
        public string SolutionContext;
        public List<string> Hints;
    }

    [Serializable]
    public sealed class ItemData
    {
        public string Id;
        public string Name;
        public string Description;
        public string Location;
        public string DiscoveryMethod;
        public string HintDirection;
    }
}
