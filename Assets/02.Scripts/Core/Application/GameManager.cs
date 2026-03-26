using System;
using System.Collections.Generic;
using _02.Scripts.Core.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.Core.Application
{
    public class GameManager : IGameManager
    {
        private readonly Dictionary<int, string> _chapterSceneMap;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public int CurrentChapter { get; private set; } = 1;
        
        public event Action<GameState> OnGameStateChanged;

        public GameManager(Dictionary<int, string> chapterSceneMap)
        {
            _chapterSceneMap = chapterSceneMap;
        }

        public void GameOver()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGameStateChanged?.Invoke(GameState.GameOver);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGameStateChanged?.Invoke(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnGameStateChanged?.Invoke(GameState.Playing);
        }

        public void RestartCurrentChapter()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            if (_chapterSceneMap.TryGetValue(CurrentChapter, out var sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void LoadChapter(int chapter)
        {
            if (!_chapterSceneMap.TryGetValue(chapter, out var sceneName)) return;
            
            CurrentChapter = chapter;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}