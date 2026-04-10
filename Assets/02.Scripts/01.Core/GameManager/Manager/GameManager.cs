using _02.Scripts._01.Core.SceneTransition.Domain;
using _02.Scripts._01.Core.SceneTransition.Manager;
using System;
using System.Collections.Generic;
using _02.Scripts.Core.Domain;
using UnityEngine;

namespace _02.Scripts.Core.Manager
{
    public class GameManager : IGameManager
    {
        private readonly List<SceneDataSO> _chapterSceneList;
        private readonly ISceneTransitionManager _sceneTransitionManager;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public int CurrentChapter { get; private set; } = 0;
        
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnChapterCleared;

        public GameManager(List<SceneDataSO> chapterSceneList, ISceneTransitionManager sceneTransitionManager)
        {
            _chapterSceneList = chapterSceneList;
            _sceneTransitionManager = sceneTransitionManager;
        }

        public void GameOver()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;
            SoundManager.Instance?.PauseAll();
            OnGameStateChanged?.Invoke(GameState.GameOver);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            SoundManager.Instance?.PauseAll();
            OnGameStateChanged?.Invoke(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;

            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            SoundManager.Instance?.ResumeAll();
            OnGameStateChanged?.Invoke(GameState.Playing);
        }

        public void RestartCurrentChapter()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            SoundManager.Instance?.ResumeAll();
            _sceneTransitionManager.RestartCurrentScene();
        }

        public void LoadChapter(int chapter)
        {
            var sceneData = _chapterSceneList.Find(s => s.ChapterId == chapter);
            if (sceneData == null) return;

            CurrentChapter = chapter;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            SoundManager.Instance?.ResumeAll();
            _sceneTransitionManager.LoadScene(sceneData);
        }

        public void CompleteChapter()
        {
            OnChapterCleared?.Invoke(CurrentChapter);
            LoadChapter(CurrentChapter + 1);
        }

        public void ReturnToMainMenu()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            SoundManager.Instance?.ResumeAll();
            _sceneTransitionManager.ReturnToMainMenu();
        }
    }
}