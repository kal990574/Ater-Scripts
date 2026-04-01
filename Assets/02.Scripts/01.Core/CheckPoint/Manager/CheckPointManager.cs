using _02.Scripts._01.Core.SceneTransition.Manager;
using _02.Scripts._03.Outgame.SaveSystem.Domain;          
using _02.Scripts._03.Outgame.SaveSystem.Manager;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;

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

        private void OnSceneLoaded()
        {
            var data = CollectSaveData();
            _saveManager.SaveGame(data,
                onSuccess: () => UnityEngine.Debug.Log("체크포인트 저장 완료"),
                onFailure: (error) => UnityEngine.Debug.LogError($"체크포인트 저장 실패: {error}")
            );
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