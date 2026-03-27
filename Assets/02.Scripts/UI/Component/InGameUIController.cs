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
        [SerializeField] private GameObject _pauseRoot;
        [SerializeField] private MainPanelManager _pausePanelManager;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _settingsPanel;
        
        [Header("Modals")]
        [SerializeField] private ModalWindowManager _gameOverModal;
        [SerializeField] private ModalWindowManager _confirmMainMenuModal;
        [SerializeField] private ModalWindowManager _confirmExitModal;
        
        private IUIManager _uiManager;
        private IGameManager _gameManager;
        private InputAction _pauseAction;

        private void Awake()
        {
            _pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        }
        
        private void Start()
        {
            _uiManager = ServiceLocator.Get<IUIManager>();
            _gameManager = ServiceLocator.Get<IGameManager>();
            _uiManager.OnUIStateChanged += HandleUIStateChanged;

            ApplyState(_uiManager.CurrentState);
        }
        
        private void OnEnable()
        {
            _pauseAction.performed += OnPausePerformed;
            _pauseAction.Enable();
        }
        
        private void OnDisable()
        {
            _pauseAction.performed -= OnPausePerformed;
            _pauseAction.Disable();
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

            if (_uiManager.CurrentState == UIState.Paused &&
                _settingsPanel.activeInHierarchy)
            {
                _pausePanelManager.OpenPanel("Pause");
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
            _hudPanel.SetActive(true);
            HidePauseRoot();
            SetCursor(false);
        }
        
        private void ShowPause()
        {
            _hudPanel.SetActive(false);
            ShowPauseRoot();
            SetCursor(true);
        }

        private void ShowGameOver()
        {
            _hudPanel.SetActive(false);
            HidePauseRoot();
            _gameOverModal.ModalWindowIn();
            SetCursor(true);
        }

        private void ShowPauseRoot()
        {
            _pauseRoot.SetActive(true);
        }

        private void HidePauseRoot()
        {
            if (!_pauseRoot.activeSelf) return;

            _pausePanelManager.currentPanelIndex = 0;
            _pausePanel.SetActive(true);
            _settingsPanel.SetActive(false);
            _pauseRoot.SetActive(false);
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