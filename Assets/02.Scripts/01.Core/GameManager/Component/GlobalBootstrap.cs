using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
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

            Managers.Register<ISceneTransitionManager>(_sceneTransitionManager);
            var gameManager = new GameManager(_chapterSceneList, _sceneTransitionManager);
            Managers.Register<IGameManager>(gameManager);
        }

        private void OnDestroy()
        {
            if (_instance != this) return;
            _instance = null;
            Managers.Clear();
        }
    }
}