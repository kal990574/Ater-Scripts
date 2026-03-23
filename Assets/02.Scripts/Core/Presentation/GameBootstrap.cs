using System.Collections.Generic;
using UnityEngine;
using _02.Scripts.Core.Application;
using _02.Scripts.Core.Domain;
using _02.Scripts.Core.Infrastructure;

namespace _02.Scripts.Core.Presentation
{
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // 실제 챕터 연동
            var chapterSceneMap = new Dictionary<int, string> { { 1, "PlayerScene" } };
            
            var gameManager = new GameManager(chapterSceneMap);
            var gameStateProvider = new GameStateProviderAdapter(gameManager);
            
            ServiceLocator.Register<IGameManager>(gameManager);
            ServiceLocator.Register<IGameStateProvider>(gameStateProvider);
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
        }
    }
}