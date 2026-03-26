using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Michsky.UI.Dark;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;

namespace _02.Scripts.UI.Component
{
    public class InGameUIController : MonoBehaviour
    {
        [Header("In-Game UI")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private GameObject _pausePanel;

        [Header("Dark UI")]
        [SerializeField] private ModalWindowManager _gameOverModal;
        [SerializeField] private ModalWindowManager _confirmQuitModal;

        private IUIManager _uiManager;
        private IGameManager _gameManager;

        private void Start()
        {
            _uiManager = ServiceLocator.Get<IUIManager>();
            _gameManager = ServiceLocator.Get<IGameManager>();
            _uiManager.OnUIStateChanged += HandleUIStateChanged;

            ApplyState(_uiManager.CurrentState);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        private void OnDestroy()
        {
            if (_uiManager != null)
            {
                _uiManager.OnUIStateChanged -= HandleUIStateChanged;
            }
        }

        private void TogglePause()
        {
            if (_uiManager.CurrentState == UIState.InGame)
            {
                _gameManager.PauseGame();
            }
            else if (_uiManager.CurrentState == UIState.Paused)
            {
                _gameManager.ResumeGame();
            }
        }

        private void HandleUIStateChanged(UIState state)
        {
            ApplyState(state);
        }

        private void ApplyState(UIState state)
        {
            switch (state)
            {
                case UIState.InGame:
                    ShowHUD();
                    break;
                case UIState.Paused:
                    ShowPause();
                    break;
                case UIState.GameOver:
                    ShowGameOver();
                    break;
            }
        }

        private void ShowHUD()
        {
            _hudPanel.SetActive(true);
            _pausePanel.SetActive(false);
            SetCursor(false);
        }

        private void ShowPause()
        {
            _hudPanel.SetActive(false);
            _pausePanel.SetActive(true);
            SetCursor(true);
        }

        private void ShowGameOver()
        {
            _hudPanel.SetActive(false);
            _gameOverModal.ModalWindowIn();
            SetCursor(true);
        }

        private void SetCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void OnResumeClicked()
        {
            _gameManager.ResumeGame();
        }

        public void OnRetryClicked()
        {
            _gameOverModal.ModalWindowOut();
            _gameManager.RestartCurrentChapter();
        }

        public void OnQuitToMenuClicked()
        {
            _confirmQuitModal.ModalWindowIn();
        }

        public void OnQuitConfirmed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("JH_MainScene");
        }

        public void OnQuitCanceled()
        {
            _confirmQuitModal.ModalWindowOut();
        }
    }
}
