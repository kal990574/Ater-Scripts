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
    public IDetectable Target => GetAbility<PlayerDetectAbility>().CurrentTarget;

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
        if (TryHandleBlockedModeInput())
        {
            return;
        }

        HandleGameplayInteractionInput();
        HandleModeSwitchInput();
        HandleCurrentModeInput();
    }

    private bool TryHandleBlockedModeInput()
    {
        switch (_interactMode)
        {
            case PlayerInteractMode.UI:
                HandleUIModeInput();
                return true;
            case PlayerInteractMode.Puzzle:
                HandlePuzzleModeInput();
                return true;
            default:
                return false;
        }
    }

    private void HandleUIModeInput()
    {
        if (_input.InventoryToggleInput)
        {
            ToggleInventoryUI();
        }
    }

    private void HandleGameplayInteractionInput()
    {
        if (Target != null && _input.InteractInput)
        {
            GetAbility<PlayerInteractAbility>().Interact(Target);
        }
    }

    private void HandleModeSwitchInput()
    {
        if (_input.ModeToggleInput)
        {
            SwitchToScanMode();
        }

        if (_input.InventoryToggleInput)
        {
            ToggleInventoryUI();
        }

        int itemSlotIndex = _input.ItemSlotInput;
        if (itemSlotIndex >= 0 && itemSlotIndex <= 5)
        {
            SwitchToItemMode(itemSlotIndex);
        }
    }

    private void HandleCurrentModeInput()
    {
        switch (_interactMode)
        {
            case PlayerInteractMode.Scan:
                ScanModeInput();
                break;
            case PlayerInteractMode.Item:
                HandleItemModeInput();
                break;
        }

        float scroll = Input.ScrollInput;
        if(!Mathf.Approximately(scroll, 0f))
        {
            SetActionMode(PlayerInteractMode.Item);
            int direction = scroll > 0f ? 1 : -1;
            GetAbility<PlayerInventoryAbility>().CycleHandItem(direction);
        }
    }

    private void HandleItemModeInput()
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

    private void SwitchToScanMode()
    {
        GetAbility<PlayerInventoryAbility>().ClearHandItem();
        SetActionMode(PlayerInteractMode.Scan);
    }

    private void ToggleInventoryUI()
    {
        bool wasInUIMode = _interactMode == PlayerInteractMode.UI;
        GetAbility<PlayerInventoryAbility>().ToggleInventory();

        if (wasInUIMode)
        {
            ExitUIMode();
            return;
        }

        EnterUIMode();
    }

    private void SwitchToItemMode(int itemSlotIndex)
    {
        SetActionMode(PlayerInteractMode.Item);
        GetAbility<PlayerInventoryAbility>().TryPickUpItem(itemSlotIndex);
    }

    private void ScanModeInput()
    {
        PlayerScanAbility scanAbility = GetAbility<PlayerScanAbility>();
        if (_input.RmbPressInput)
        {
            scanAbility.SonarActive();
        }

        if (_input.LmbPressInput)
        {
            scanAbility.LidarScanActive();
        }
        
        if (_input.LmbHoldInput)
        {
            scanAbility.LidarScanUpdate();
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
