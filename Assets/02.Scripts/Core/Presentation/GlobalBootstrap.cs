using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Core.Application;
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using _02.Scripts.UI.Manager;

namespace _02.Scripts.Core.Presentation
{
    [DefaultExecutionOrder(-100)]
    public class GlobalBootstrap : MonoBehaviour
    {
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
            var chapterSceneMap = new Dictionary<int, string>
            {
                { 1, "PlayerScene" }
            };

            var gameManager = new GameManager(chapterSceneMap, "JH_MainScene");
            ServiceLocator.Register<IGameManager>(gameManager);
        }

        private void OnDestroy()
        {
            _initialized = false;
            ServiceLocator.Clear();
        }
    }
}