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
            CurrentState = UIState.InGame;
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
                    SetState(UIState.InGame);
                    break;
                case GameState.Paused:
                    SetState(UIState.Paused);
                    break;
                case GameState.GameOver:
                    SetState(UIState.GameOver);
                    break;
            }
        }
    }
}