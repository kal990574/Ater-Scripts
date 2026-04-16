using System;
using _02.Scripts.Core.Domain;
using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerModeService
    {
        private EPlayerInteractMode _currentMode;
        private EPlayerInteractMode _lastGameplayMode;
        private bool _canMove;
        private bool _canRotate;
        private bool _isPausedByGame;
        private IPuzzleInputHandler _activePuzzleInputHandler;

        public PlayerModeService(EPlayerInteractMode initialMode)
        {
            _currentMode = initialMode;
            _lastGameplayMode = IsGameplayMode(initialMode) ? initialMode : EPlayerInteractMode.Scan;
            ApplyModeState();
        }

        public EPlayerInteractMode CurrentMode => _currentMode;
        public bool CanMove => _canMove;
        public bool CanRotate => _canRotate;
        public bool IsPausedByGame => _isPausedByGame;
        public bool BlocksGameplayInput => _currentMode == EPlayerInteractMode.UI
            || _currentMode == EPlayerInteractMode.Puzzle
            || _currentMode == EPlayerInteractMode.Cutscene;
        public IPuzzleInputHandler ActivePuzzleInputHandler => _activePuzzleInputHandler;

        public event Action<EPlayerInteractMode> OnModeChanged;

        public void EnterCutsceneMode()
        {
            SetMode(EPlayerInteractMode.Cutscene);
        }

        public void ExitCutsceneMode()
        {
            SetMode(_lastGameplayMode);
        }

        public void EnterUIMode()
        {
            SetMode(EPlayerInteractMode.UI);
        }

        public void ExitUIMode()
        {
            SetMode(_lastGameplayMode);
        }

        public void EnterPuzzleMode(IPuzzleInputHandler puzzleInputHandler)
        {
            _activePuzzleInputHandler = puzzleInputHandler;
            SetMode(EPlayerInteractMode.Puzzle);
        }

        public void ExitPuzzleMode(IPuzzleInputHandler puzzleInputHandler)
        {
            if (_activePuzzleInputHandler != null && _activePuzzleInputHandler != puzzleInputHandler)
            {
                return;
            }

            _activePuzzleInputHandler = null;
            SetMode(_lastGameplayMode);
        }

        public void SetGameplayMode(EPlayerInteractMode mode)
        {
            if (!IsGameplayMode(mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "Only gameplay modes are allowed.");
            }

            SetMode(mode);
        }

        public void HandleInventoryToggled(bool isOn)
        {
            if (isOn)
            {
                EnterUIMode();
                return;
            }

            if (_currentMode == EPlayerInteractMode.UI)
            {
                ExitUIMode();
            }
        }

        public void HandleGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                case GameState.GameOver:
                    _isPausedByGame = true;
                    _canMove = false;
                    _canRotate = false;
                    break;
                case GameState.Playing:
                    _isPausedByGame = false;
                    ApplyModeState();
                    break;
            }
        }

        private void SetMode(EPlayerInteractMode mode)
        {
            if (_currentMode == mode)
            {
                return;
            }

            if (IsGameplayMode(mode))
            {
                _lastGameplayMode = mode;
            }

            _currentMode = mode;
            ApplyModeState();
            OnModeChanged?.Invoke(mode);
        }

        private void ApplyModeState()
        {
            if (_isPausedByGame)
            {
                return;
            }

            bool blocksPlayerControl = _currentMode == EPlayerInteractMode.UI
                || _currentMode == EPlayerInteractMode.Puzzle
                || _currentMode == EPlayerInteractMode.Cutscene;

            _canMove = !blocksPlayerControl;
            _canRotate = !blocksPlayerControl;

            bool showCursor = _currentMode == EPlayerInteractMode.UI
                || _currentMode == EPlayerInteractMode.Puzzle;

            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = showCursor;
        }

        private static bool IsGameplayMode(EPlayerInteractMode mode)
        {
            return mode == EPlayerInteractMode.Item || mode == EPlayerInteractMode.Scan;
        }
    }
}
