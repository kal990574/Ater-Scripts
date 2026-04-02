using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts._03.Outgame.SaveSystem.Domain
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private List<int> _clearedChapters;

        public List<int> ClearedChapters => _clearedChapters;

        public SaveData()
        {
            _clearedChapters = new List<int>();
        }
    }
}