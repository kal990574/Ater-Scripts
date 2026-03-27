using UnityEngine;
using UnityEngine.InputSystem;
using Michsky.UI.Dark;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using _02.Scripts.UI.Domain;

namespace _02.Scripts.UI.Component
{
    public class InGameUIController : MonoBehaviour
    {
        [Header("HUD")] 
        [SerializeField] private GameObject _hudPanel;
        
        [Header("Pause System")]
        [SerializeField] private MainPanelManager _panelManager;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _settingsPanel;
        
        [Header("Modals")]
        [SerializeField] private ModalWindowManager _gameOverModal;
        [SerializeField] private ModalWindowManager _confirmMainMenuModal;
        [SerializeField] private ModalWindowManager _confirmExitModal;
        
        [Header("Input")]
        [SerializeField] private InputActionReference _pauseAction;
        
        private IUIManager _uiManager;
        private IGameManager _gameManager;
        private void Start()
        {
            _uiManager = Managers.Get<IUIManager>();
            _gameManager = Managers.Get<IGameManager>();
            _uiManager.OnUIStateChanged += HandleUIStateChanged;

            ApplyState(_uiManager.CurrentState);
        }
        
        private void OnEnable()
        {
            _pauseAction.action.performed += OnPausePerformed;
            _pauseAction.action.Enable();
        }
        
        private void OnDisable()
        {
            _pauseAction.action.performed -= OnPausePerformed;
            _pauseAction.action.Disable();
        }

        private void OnDestroy()
        {
            if (_uiManager != null)
            {
                _uiManager.OnUIStateChanged -= HandleUIStateChanged;
            }
        }
        
        // --- Input ---
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (_confirmMainMenuModal.isOn)
            {
                _confirmMainMenuModal.ModalWindowOut();
                return;
            }

            if (_confirmExitModal.isOn)
            {
                _confirmExitModal.ModalWindowOut();
                return;
            }

            if (_uiManager.CurrentState == UIState.GameOver) return;

            if (_uiManager.CurrentState == UIState.Paused && _settingsPanel.activeInHierarchy)
            {
                _panelManager.OpenPanel("Pause");
                return;
            }

            TogglePause();
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
        
        // --- State ---
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
            _panelManager.OpenPanel("HUD");
            SetCursor(false);
        }
        
        private void ShowPause()
        {
            _panelManager.OpenPanel("Pause");
            SetCursor(true);
        }

        private void ShowGameOver()
        {
            _gameOverModal.ModalWindowIn();
            SetCursor(true);
        }

        private void SetCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        // --- Pause Panel Callbacks ---
        public void OnResumeClicked()
        {
            _gameManager.ResumeGame();
        }

        public void OnMainMenuClicked()
        {
            _confirmMainMenuModal.ModalWindowIn();
        }

        public void OnExitClicked()
        {
            _confirmExitModal.ModalWindowIn();
        }
        
        // --- GameOver Modal Callbacks ---
        public void OnRestartClicked()
        {
            _gameOverModal.ModalWindowOut();
            _gameManager.RestartCurrentChapter();
        }
        
        // --- Confirm Main Menu Modal ---
        public void OnMainMenuConfirmed()
        {
            _gameManager.ReturnToMainMenu();
        }

        public void OnMainMenuCanceled()
        {
            _confirmMainMenuModal.ModalWindowOut();
        }
        
        // --- Confirm Exit Modal ---

        public void OnExitConfirmed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
              Application.Quit();
#endif
        }

        public void OnExitCanceled()
        {
            _confirmExitModal.ModalWindowOut();
        }
    }
}