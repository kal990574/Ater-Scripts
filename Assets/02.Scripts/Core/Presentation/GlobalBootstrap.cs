using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Core.Application;
using _02.Scripts.Core.Domain;
using _02.Scripts.Core.Infrastructure;

namespace _02.Scripts.Core.Presentation
{
    [DefaultExecutionOrder(-100)]
    public class GlobalBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // 실제 글로벌 매니저들 연동할 부분
            var chapterSceneMap = new Dictionary<int, string>
            {
                { 1, "PlayerScene" }
            };

            var gameManager = new GameManager(chapterSceneMap);
            ServiceLocator.Register<IGameManager>(gameManager);
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}