using System;

namespace _02.Scripts.UI.Domain
{
    public interface IUIManager
    {
        UIState CurrentState { get; }
        event Action<UIState> OnUIStateChange;

        void ShowMainMenu();
        void ShowInGameHUD();
        void ShowPauseMenu();
        void ShowGameOver();
        void ShowSettings(UIState returnTo);
    }
}