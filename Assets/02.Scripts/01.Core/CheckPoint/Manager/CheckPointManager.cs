using _02.Scripts._03.Outgame.SaveSystem.Domain;
using _02.Scripts._03.Outgame.SaveSystem.Manager;
using _02.Scripts.Core.Domain;
using System;
using UnityEngine;

namespace _02.Scripts._01.Core.CheckPoint.Manager
{
    public class CheckPointManager
    {
        private readonly SaveManager _saveManager;
        private readonly IGameManager _gameManager;

        public CheckPointManager(SaveManager saveManager, IGameManager gameManager)
        {
            _saveManager = saveManager;
            _gameManager = gameManager;

            _gameManager.OnChapterCleared += OnChapterCleared;
        }

        private async void OnChapterCleared(int chapter)
        {
            try
            {
                SaveData data;
                try { data = await _saveManager.LoadGame(); }
                catch { data = new SaveData(); }

                if (!data.ClearedChapters.Contains(chapter))
                    data.ClearedChapters.Add(chapter);

                await _saveManager.SaveGame(data);
                Debug.Log($"챕터 {chapter} 클리어 저장 완료");
            }
            catch (Exception e)
            {
                Debug.LogError($"클리어 저장 실패: {e.Message}");
            }
        }

        public void Dispose()
        {
            _gameManager.OnChapterCleared -= OnChapterCleared;
        }
    }
}