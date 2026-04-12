using System;
namespace _02.Scripts.Core.Domain
{
    public interface IGameManager
    {
        GameState CurrentState { get; }
        int CurrentChapter { get; }

        event Action<GameState> OnGameStateChanged;
        event Action<int> OnChapterCleared;

        void GameOver();
        void RestartCurrentChapter();
        void LoadChapter(int chapter);
        void CompleteChapter();
        void PauseGame();
        void ResumeGame();
        void MarkChapterCleared();
        void ReturnToMainMenu();
        void EnterTransition();
        void ExitTransition();
    }
}