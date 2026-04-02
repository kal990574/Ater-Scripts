using _02.Scripts._01.Core.CheckPoint.Manager;
using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
using _02.Scripts._03.Outgame.SaveSystem.Manager;
using _02.Scripts._03.Outgame.SaveSystem.Repository;
using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Core.Manager;
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;

namespace _02.Scripts.Core.Component
{
    [DefaultExecutionOrder(-100)]
    public class GlobalBootstrap : MonoBehaviour
    {
        [SerializeField] private List<SceneDataSO> _chapterSceneList;
        [SerializeField] private SceneTransitionManager _sceneTransitionManager;
        
        private static GlobalBootstrap _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Scene Transition Manager
            Managers.Register<ISceneTransitionManager>(_sceneTransitionManager);
            
            // Game Manager
            var gameManager = new GameManager(_chapterSceneList, _sceneTransitionManager);
            Managers.Register<IGameManager>(gameManager);
            
            // Save Manager
            var saveRepository = new LocalSaveRepository();
            var saveManager = new SaveManager(saveRepository);
            Managers.Register<SaveManager>(saveManager);
            
            // CheckPoint Manager
            var checkPointManager = new CheckPointManager(saveManager, _sceneTransitionManager);
            Managers.Register<CheckPointManager>(checkPointManager);
        }

        private void OnDestroy()
        {
            if (_instance != this) return;
            _instance = null;
            Managers.Clear();
        }
    }
}