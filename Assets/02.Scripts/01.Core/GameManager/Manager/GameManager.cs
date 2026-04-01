using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
using System;
using System.Collections.Generic;
using _02.Scripts.Core.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.Core.Manager
{
    public class GameManager : IGameManager
    {
        private readonly List<SceneDataSO> _chapterSceneList;
        private readonly ISceneTransitionManager _sceneTransition;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public int CurrentChapter { get; private set; } = 0;
        
        public event Action<GameState> OnGameStateChanged;

        public GameManager(List<SceneDataSO> chapterSceneList, ISceneTransitionManager sceneTransition)
        {
            _chapterSceneList = chapterSceneList;
            _sceneTransition = sceneTransition;
        }

        public void GameOver()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0;
            OnGameStateChanged?.Invoke(GameState.GameOver);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            OnGameStateChanged?.Invoke(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            OnGameStateChanged?.Invoke(GameState.Playing);
        }

        public void RestartCurrentChapter()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            _sceneTransition.RestartCurrentScene();
        }

        public void LoadChapter(int chapter)
        {
            var sceneData = _chapterSceneList.Find(s => s.ChapterId == chapter);
            if (sceneData == null) return;
            
            CurrentChapter = chapter;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            _sceneTransition.LoadScene(sceneData);
        }

        public void ReturnToMainMenu()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            _sceneTransition.ReturnToMainMenu();
        }
    }
}