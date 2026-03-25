using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;
using System;

namespace _02.Scripts.UI.Manager
{
    public class UIManager : IUIManager
    {
        private readonly IGameManager _gameManager;
        public UIState CurrentState { get; private set; }
        public event Action<UIState> OnUIStateChanged;

        public UIManager(IGameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnGameStateChanged += HandleGameStateChanged;
            CurrentState = UIState.MainMenu;
        }
        
        public void ShowMainMenu()
        {
            SetState(UIState.MainMenu);
        }

        public void ShowInGameHUD()
        {
            SetState(UIState.InGame);
        }

        public void ShowPauseMenu()
        {
            SetState(UIState.Paused);
        }

        public void ShowGameOver()
        {
            SetState(UIState.GameOver);
        }

        private void SetState(UIState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnUIStateChanged?.Invoke(newState);
        }

        private void HandleGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    ShowInGameHUD();
                    break;
                case GameState.Paused:
                    ShowPauseMenu();
                    break;
                case GameState.GameOver:
                    ShowGameOver();
                    break;
            }
        }
    }
}