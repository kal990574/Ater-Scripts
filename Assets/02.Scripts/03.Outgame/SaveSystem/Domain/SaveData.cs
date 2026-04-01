using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts._03.Outgame.SaveSystem.Domain
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int _chapterId;
        [SerializeField] private string _savedAt;
        
        public int ChapterId => _chapterId;
        public string SavedAt {get => _savedAt; set => _savedAt = value; }

        public SaveData(int chapterId)
        {
            _chapterId = chapterId;
        }
        // ToDo: 쪽지 오브젝트도 저장할 예정
    }
}