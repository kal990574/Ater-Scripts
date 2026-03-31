using System;
namespace _02.Scripts.Core.Domain
{
    public interface IGameManager
    {
        GameState CurrentState { get; }
        int CurrentChapter { get; }

        event Action<GameState> OnGameStateChanged;

        void GameOver();
        void RestartCurrentChapter();
        void LoadChapter(int chapter);
        void PauseGame();
        void ResumeGame();
        void ReturnToMainMenu();

    }
}