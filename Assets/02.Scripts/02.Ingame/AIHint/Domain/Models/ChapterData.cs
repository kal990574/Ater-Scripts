using System;
using System.Collections.Generic;

namespace _02.Scripts.AIHint.Domain.Models
{
    [Serializable]
    public sealed class ChapterData
    {
        public int Chapter;
        public string Name;
        public List<TaskData> Tasks;
        public List<ItemHintData> Items;
    }

    [Serializable]
    public sealed class TaskData
    {
        public string Task;
        public List<string> Requires;
        public string Produces;
        public string Hint;
    }

    [Serializable]
    public sealed class ItemHintData
    {
        public string Name;
        public string HintDirection;
    }
}
