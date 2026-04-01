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
        
        private static bool _initialized;
        private void Awake()
        {
            if (_initialized)
            {
                Destroy(gameObject);
                return;
            }
            _initialized = true;
            DontDestroyOnLoad(gameObject);
            
            // 실제 글로벌 매니저들 연동할 부분
            Managers.Register<ISceneTransitionManager>(_sceneTransitionManager);
            var gameManager = new GameManager(_chapterSceneList, _sceneTransitionManager);
            Managers.Register<IGameManager>(gameManager);
        }

        private void OnDestroy()
        {
            _initialized = false;
            Managers.Clear();
        }
    }
}