using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInteractMode
{
    Item,
    Scan,
    UI,
    Puzzle,
}

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfigSO _playerConfig;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canRotate = true;
    [SerializeField] private PlayerInteractMode _interactMode = PlayerInteractMode.Scan;

    private readonly Dictionary<Type, PlayerAbility> _abilities = new();
    private IPlayerInput _input;
    private PlayerInteractMode _lastGameplayMode = PlayerInteractMode.Scan;
    private PadLockController _activePadLockController;

    public PlayerConfigSO Config => _playerConfig;
    public IPlayerInput Input => _input;
    public bool CanMove => _canMove;
    public bool CanRotate => _canRotate;
    public PlayerInteractMode InteractMode => _interactMode;
    public IDetectableObject Target => GetAbility<PlayerDetectAbility>().CurrentTarget;

    public event Action<PlayerInteractMode> OnModeChanged;

    private void Awake()
    {
        if (_input == null)
        {
            _input = GetComponentInChildren<IPlayerInput>();
        }

        if (IsGameplayMode(_interactMode))
        {
            _lastGameplayMode = _interactMode;
        }

        ApplyModeState(_interactMode);
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryToggled += HandleInventoryToggled;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryToggled -= HandleInventoryToggled;
        }
    }

    private void Update()
    {
        if (_interactMode == PlayerInteractMode.UI)
        {
            if (_input.InventoryToggleInput)
            {
                GetAbility<PlayerInventoryAbility>().ToggleInventory();
            }

            return;
        }

        if (_interactMode == PlayerInteractMode.Puzzle)
        {
            HandlePuzzleModeInput();
            return;
        }

        if (Target != null && _input.InteractInput)
        {
            GetAbility<PlayerInteractAbility>().Interact(Target);
        }

        if (_interactMode == PlayerInteractMode.Scan)
        {
            ScanModeInput();
        }

        if (_input.ModeToggleInput)
        {
            GetAbility<PlayerInventoryAbility>().ClearHandItem();
            SetActionMode(PlayerInteractMode.Scan);
        }

        if (_input.InventoryToggleInput)
        {
            GetAbility<PlayerInventoryAbility>().ToggleInventory();
        }

        if (_input.ItemSlotInput >= 0 && _input.ItemSlotInput <= 5)
        {
            SetActionMode(PlayerInteractMode.Item);
            GetAbility<PlayerInventoryAbility>().TryPickUpItem(_input.ItemSlotInput);
        }

        if (_interactMode == PlayerInteractMode.Item)
        {
            PlayerInventoryAbility inventoryAbility = GetAbility<PlayerInventoryAbility>();
            if (_input.LmbPressInput)
            {
                inventoryAbility.BeginReleaseHandItem();
            }

            if (_input.LmbHoldInput)
            {
                inventoryAbility.ChargeReleaseHandItem(Time.deltaTime);
            }

            if (_input.LmbReleaseInput)
            {
                inventoryAbility.ReleaseHandItem();
            }
        }
    }

    private void ScanModeInput()
    {
        PlayerScanAbility scanAbility = GetAbility<PlayerScanAbility>();
        if (_input.RmbPressInput)
        {
            scanAbility.SonarActive();
        }

        if (_input.LmbPressingInput)
        {
            scanAbility.LidarScanActiveAndUpdate();
        }

        if (_input.InteractInput)
        {
            QTEManager.Instance?.SubmitCurrent();
        }

        if (_input.LmbReleaseInput)
        {
            scanAbility.LidarScanDeactive();
        }
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        Type type = typeof(T);

        if (_abilities.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        ability = GetComponentInChildren<T>();
        if (ability != null)
        {
            _abilities[type] = ability;
            return ability as T;
        }

        return null;
    }

    public void SetActionMode(PlayerInteractMode mode)
    {
        if (_interactMode == mode)
        {
            return;
        }

        if (IsGameplayMode(mode))
        {
            _lastGameplayMode = mode;
        }

        _interactMode = mode;
        ApplyModeState(mode);
        OnModeChanged?.Invoke(mode);
    }

    public void EnterUIMode()
    {
        SetActionMode(PlayerInteractMode.UI);
    }

    public void ExitUIMode()
    {
        SetActionMode(_lastGameplayMode);
    }

    public void EnterPuzzleMode()
    {
        SetActionMode(PlayerInteractMode.Puzzle);
    }

    public void ExitPuzzleMode()
    {
        SetActionMode(_lastGameplayMode);
    }

    public void EnterPuzzleMode(PadLockController padLockController)
    {
        _activePadLockController = padLockController;
        EnterPuzzleMode();
    }

    public void ExitPuzzleMode(PadLockController padLockController)
    {
        if (_activePadLockController != null && _activePadLockController != padLockController)
        {
            return;
        }

        _activePadLockController = null;
        ExitPuzzleMode();
    }

    private void HandleInventoryToggled(bool isOn)
    {
        if (isOn)
        {
            EnterUIMode();
            return;
        }

        if (_interactMode == PlayerInteractMode.UI)
        {
            ExitUIMode();
        }
    }

    private void HandlePuzzleModeInput()
    {
        if (_input.ConfirmInput)
        {
            _activePadLockController?.ConfirmActivePuzzle();
        }

        if (_input.CancelInput)
        {
            _activePadLockController?.CancelActivePuzzle();
        }
    }

    private void ApplyModeState(PlayerInteractMode mode)
    {
        bool blocksPlayerControl = mode == PlayerInteractMode.UI || mode == PlayerInteractMode.Puzzle;
        _canMove = !blocksPlayerControl;
        _canRotate = !blocksPlayerControl;

        Cursor.lockState = blocksPlayerControl ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blocksPlayerControl;
    }

    private static bool IsGameplayMode(PlayerInteractMode mode)
    {
        return mode == PlayerInteractMode.Item || mode == PlayerInteractMode.Scan;
    }
}
