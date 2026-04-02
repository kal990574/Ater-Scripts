using _02.Scripts._01.Core.SceneTransition.Manager;
using _02.Scripts._03.Outgame.SaveSystem.Domain;          
using _02.Scripts._03.Outgame.SaveSystem.Manager;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using System;
using UnityEngine;

namespace _02.Scripts._01.Core.CheckPoint.Manager
{
    public class CheckPointManager
    {
        private readonly SaveManager _saveManager;
        private readonly ISceneTransitionManager _sceneTransitionManager;

        public CheckPointManager(SaveManager saveManager, ISceneTransitionManager sceneTransitionManager)
        {
            _saveManager = saveManager;
            _sceneTransitionManager = sceneTransitionManager;

            _sceneTransitionManager.OnTransitionCompleted += OnSceneLoaded;
        }

        private async void OnSceneLoaded()
        {
            try
            {
                var data = CollectSaveData();
                await _saveManager.SaveGame(data);
                Debug.Log("체크포인트 저장 완료");
            }
            catch (Exception e)
            {
                Debug.LogError($"체크포인트 저장 실패 : {e.Message}");
            }
        }

        private SaveData CollectSaveData()
        {
            // TODO: 상태 수집
            var gameManager = Managers.Get<IGameManager>();
            return new SaveData(gameManager.CurrentChapter);
        }

        public void Dispose()
        {
            _sceneTransitionManager.OnTransitionCompleted -= OnSceneLoaded;
        }
    }
}