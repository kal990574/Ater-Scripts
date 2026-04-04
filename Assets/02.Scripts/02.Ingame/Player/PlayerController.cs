using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour ,IPlayerModeProvider
{
    [Header("References")]
    [SerializeField] private PlayerConfigSO _playerConfig;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canRotate = true;
    [SerializeField] private EPlayerInteractMode _interactMode = EPlayerInteractMode.Scan;

    private readonly Dictionary<Type, PlayerAbility> _abilities = new();
    private IPlayerInput _input;
    private EPlayerInteractMode _lastGameplayMode = EPlayerInteractMode.Scan;
    private PadLockController _activePadLockController;

    public PlayerConfigSO Config => _playerConfig;
    public IPlayerInput Input => _input;
    public bool CanMove => _canMove;
    public bool CanRotate => _canRotate;
    public EPlayerInteractMode InteractMode => _interactMode;
    public IDetectable Target => GetAbility<PlayerDetectAbility>().CurrentTarget;

    public event Action<EPlayerInteractMode> OnModeChanged;

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
            case EPlayerInteractMode.UI:
                HandleUIModeInput();
                return true;
            case EPlayerInteractMode.Puzzle:
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
            case EPlayerInteractMode.Scan:
                ScanModeInput();
                break;
            case EPlayerInteractMode.Item:
                HandleItemModeInput();
                break;
        }

        float scroll = Input.ScrollInput;
        if(!Mathf.Approximately(scroll, 0f))
        {
            SetInteractMode(EPlayerInteractMode.Item);
            int direction = scroll > 0f ? 1 : -1;
            GetAbility<PlayerHandAbility>().CycleHandItem(direction);
        }
    }

    private void HandleItemModeInput()
    {
        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        if (_input.LmbPressInput)
        {
            handAbility.BeginReleaseHandItem();
        }

        if (_input.LmbHoldInput)
        {
            handAbility.ChargeReleaseHandItem(Time.deltaTime);
        }

        if (_input.LmbReleaseInput)
        {
            handAbility.ReleaseHandItem();
        }
    }

    private void SwitchToScanMode()
    {
        GetAbility<PlayerHandAbility>().ClearHandItem();
        SetInteractMode(EPlayerInteractMode.Scan);
    }

    private void ToggleInventoryUI()
    {
        bool wasInUIMode = _interactMode == EPlayerInteractMode.UI;
        GetAbility<PlayerHandAbility>().ToggleInventory();

        if (wasInUIMode)
        {
            ExitUIMode();
            return;
        }

        EnterUIMode();
    }

    private void SwitchToItemMode(int itemSlotIndex)
    {
        SetInteractMode(EPlayerInteractMode.Item);
        GetAbility<PlayerHandAbility>().TryPickUpItem(itemSlotIndex);
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

    public void SetInteractMode(EPlayerInteractMode mode)
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
        SetInteractMode(EPlayerInteractMode.UI);
    }

    public void ExitUIMode()
    {
        SetInteractMode(_lastGameplayMode);
    }

    public void EnterPuzzleMode()
    {
        SetInteractMode(EPlayerInteractMode.Puzzle);
    }

    public void ExitPuzzleMode()
    {
        SetInteractMode(_lastGameplayMode);
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

        if (_interactMode == EPlayerInteractMode.UI)
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

    private void ApplyModeState(EPlayerInteractMode mode)
    {
        bool blocksPlayerControl = mode == EPlayerInteractMode.UI || mode == EPlayerInteractMode.Puzzle;
        _canMove = !blocksPlayerControl;
        _canRotate = !blocksPlayerControl;

        Cursor.lockState = blocksPlayerControl ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blocksPlayerControl;
    }

    private static bool IsGameplayMode(EPlayerInteractMode mode)
    {
        return mode == EPlayerInteractMode.Item || mode == EPlayerInteractMode.Scan;
    }

    public EPlayerInteractMode GetCurrentMode()
    {
        return _interactMode;
    }
}
