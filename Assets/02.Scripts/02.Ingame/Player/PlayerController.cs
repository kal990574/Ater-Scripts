using _02.Scripts.Core;
using _02.Scripts.Core.Domain;
using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour ,IPlayerModeProvider
{
    [Header("References")]
    [SerializeField] private PlayerConfigSO _playerConfig;
    [SerializeField] private EPlayerInteractMode _initialMode = EPlayerInteractMode.Scan;

    private readonly Dictionary<Type, PlayerAbility> _abilities = new();
    private IPlayerInput _input;
    private IGameManager _gameManager;
    private PlayerModeService _modeService;
    private PlayerInteractionContextFactory _interactionContextFactory;
    private PlayerGameplayInputRouter _gameplayInputRouter;
    private PlayerBlockedInputRouter _blockedInputRouter;
    private PlayerEventPublisher _eventPublisher;
    
    public PlayerConfigSO Config => _playerConfig;
    public IPlayerInput Input => _input;
    public bool CanMove => _modeService != null && _modeService.CanMove;
    public bool CanRotate => _modeService != null && _modeService.CanRotate;
    public EPlayerInteractMode InteractMode => _modeService != null ? _modeService.CurrentMode : _initialMode;
    public PlayerEventPublisher EventPublisher => _eventPublisher;
    private IDetectable Target => GetAbility<PlayerDetectAbility>()?.CurrentTarget;

    public event Action<EPlayerInteractMode> OnModeChanged;

    private void Awake()
    {
        if (_input == null)
        {
            _input = GetComponentInChildren<IPlayerInput>();
        }

        _modeService = new PlayerModeService(_initialMode);
        _modeService.OnModeChanged += HandleModeChanged;

        PlayerDetectAbility detectAbility = GetAbility<PlayerDetectAbility>();
        if (detectAbility != null)
        {
            _eventPublisher = new PlayerEventPublisher(this, detectAbility.PromptQuery);
            detectAbility.SetEventPublisher(_eventPublisher);
        }

        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        _interactionContextFactory = new PlayerInteractionContextFactory(this, handAbility);
        _gameplayInputRouter = new PlayerGameplayInputRouter(
            _modeService,
            () => Target,
            GetAbility<PlayerInteractAbility>(),
            GetAbility<PlayerScanAbility>(),
            handAbility,
            SwitchToItemMode,
            SwitchToScanMode,
            ToggleInventoryUI);
        _blockedInputRouter = new PlayerBlockedInputRouter(_modeService, ToggleInventoryUI);
    }

    private void OnEnable()
    {
        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        if (handAbility != null)
        {
            handAbility.OnHandSlotChanged += HandleHandSlotChanged;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryToggled += HandleInventoryToggled;
        }
        _gameManager = Managers.Get<IGameManager>();
        if (_gameManager != null)
        {
            _gameManager.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        if (handAbility != null)
        {
            handAbility.OnHandSlotChanged -= HandleHandSlotChanged;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryToggled -= HandleInventoryToggled;
        }

        if (_gameManager != null)
        {
            _gameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (_modeService != null)
        {
            _modeService.OnModeChanged -= HandleModeChanged;
        }
    }

    private void Update()
    {
        if (_input == null)
        {
            return;
        }

        if (_modeService != null && _modeService.IsPausedByGame)
        {
            return;
        }

        if (_blockedInputRouter != null && _blockedInputRouter.Handle(_input))
        {
            return;
        }

        _gameplayInputRouter?.Handle(_input);
    }

    public void EnterCutsceneMode()
    {
        _modeService?.EnterCutsceneMode();
    }

    public void ExitCutsceneMode()
    {
        _modeService?.ExitCutsceneMode();
    }
    
    public bool SwitchToScanMode()
    {
        if (InteractMode == EPlayerInteractMode.Scan)
        {
            return false;
        }

        PlayerModeTransitionAbility modeTransitionAbility = GetAbility<PlayerModeTransitionAbility>();
        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        PlayerScanAbility scanAbility = GetAbility<PlayerScanAbility>();

        if (modeTransitionAbility != null && modeTransitionAbility.TryPlayTransition(
                onLowered: () =>
                {
                    handAbility?.ClearHandItem();
                    scanAbility?.SetScannerVisible(true);
                },
                onCompleted: () => _modeService?.SetGameplayMode(EPlayerInteractMode.Scan)))
        {
            _modeService?.SetGameplayMode(EPlayerInteractMode.Scan);
            return true;
        }

        handAbility?.ClearHandItem();
        scanAbility?.SetScannerVisible(true);
        _modeService?.SetGameplayMode(EPlayerInteractMode.Scan);
        return true;
    }

    private void ToggleInventoryUI()
    {
        bool wasInUIMode = InteractMode == EPlayerInteractMode.UI;
        GetAbility<PlayerHandAbility>()?.ToggleInventory();

        if (wasInUIMode)
        {
            ExitUIMode();
            return;
        }

        EnterUIMode();
    }

    public bool SwitchToItemMode(int itemSlotIndex)
    {
        PlayerModeTransitionAbility modeTransitionAbility = GetAbility<PlayerModeTransitionAbility>();
        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        PlayerScanAbility scanAbility = GetAbility<PlayerScanAbility>();

        if (handAbility != null
            && handAbility.HasHandItem
            && handAbility.CurrentHandIndex == itemSlotIndex)
        {
            return false;
        }

        if (handAbility != null && handAbility.CanPickUpItem(itemSlotIndex) == false)
        {
            return false;
        }

        if (modeTransitionAbility != null && modeTransitionAbility.TryPlayTransition(
                onLowered: () =>
                {
                    scanAbility?.SetScannerVisible(false);
                    bool didEquip = handAbility != null && handAbility.TryPickUpItem(itemSlotIndex);
                    if (didEquip == false)
                    {
                        scanAbility?.SetScannerVisible(true);
                        _modeService?.SetGameplayMode(EPlayerInteractMode.Scan);
                    }
                },
                onCompleted: null))
        {
            _modeService?.SetGameplayMode(EPlayerInteractMode.Item);
            return true;
        }

        _modeService?.SetGameplayMode(EPlayerInteractMode.Item);
        scanAbility?.SetScannerVisible(false);

        if (handAbility == null || handAbility.TryPickUpItem(itemSlotIndex))
        {
            return true;
        }

        scanAbility?.SetScannerVisible(true);
        _modeService?.SetGameplayMode(EPlayerInteractMode.Scan);
        return false;
    }

    public void HandleHandConsumeResult(HandConsumeResult result)
    {
        if (!result.Consumed)
        {
            return;
        }

        if (result.InventoryEmptyAfterConsume || result.NextRecommendedSlotIndex < 0)
        {
            SwitchToScanMode();
            return;
        }

        _modeService?.SetGameplayMode(EPlayerInteractMode.Item);
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

    [System.Obsolete("Prefer explicit mode request methods such as EnterUIMode, EnterPuzzleMode, SwitchToScanMode, or SwitchToItemMode.")]
    public void SetInteractMode(EPlayerInteractMode mode)
    {
        if (_modeService == null)
        {
            return;
        }

        if (mode == EPlayerInteractMode.Item || mode == EPlayerInteractMode.Scan)
        {
            _modeService.SetGameplayMode(mode);
            return;
        }

        switch (mode)
        {
            case EPlayerInteractMode.UI:
                _modeService.EnterUIMode();
                break;
            case EPlayerInteractMode.Puzzle:
                _modeService.EnterPuzzleMode(null);
                break;
            case EPlayerInteractMode.Cutscene:
                _modeService.EnterCutsceneMode();
                break;
        }
    }

    public void EnterUIMode()
    {
        _modeService?.EnterUIMode();
    }

    public void ExitUIMode()
    {
        _modeService?.ExitUIMode();
    }

    public void EnterPuzzleMode()
    {
        GetAbility<PlayerDetectAbility>()?.ForceHidePrompt();
        _modeService?.EnterPuzzleMode(null);
    }

    public void ExitPuzzleMode()
    {
        GetAbility<PlayerDetectAbility>()?.ResumePrompt();
        _modeService?.ExitPuzzleMode(null);
    }

    public void EnterPuzzleMode(IPuzzleInputHandler puzzleInputHandler)
    {
        GetAbility<PlayerDetectAbility>()?.ForceHidePrompt();
        _modeService?.EnterPuzzleMode(puzzleInputHandler);
    }

    public void ExitPuzzleMode(IPuzzleInputHandler puzzleInputHandler)
    {
        if (_modeService != null
            && _modeService.ActivePuzzleInputHandler != null
            && _modeService.ActivePuzzleInputHandler != puzzleInputHandler)
        {
            return;
        }

        GetAbility<PlayerDetectAbility>()?.ResumePrompt();
        _modeService?.ExitPuzzleMode(puzzleInputHandler);
    }

    private void HandleInventoryToggled(bool isOn)
    {
        _modeService?.HandleInventoryToggled(isOn);
    }

    private void HandleGameStateChanged(GameState state)
    {
        _modeService?.HandleGameStateChanged(state);
    }

    private void HandleModeChanged(EPlayerInteractMode mode)
    {
        OnModeChanged?.Invoke(mode);
    }

    private void HandleHandSlotChanged(int handIndex)
    {
        if (InteractMode != EPlayerInteractMode.Item || handIndex >= 0)
        {
            return;
        }

        PlayerHandAbility handAbility = GetAbility<PlayerHandAbility>();
        if (handAbility == null || handAbility.InventoryCount <= 0)
        {
            SwitchToScanMode();
        }
    }

    public EPlayerInteractMode GetCurrentMode()
    {
        return InteractMode;
    }

    public InteractionContext CreateInteractionContext(GameObject targetObject)
    {
        if (_interactionContextFactory == null)
        {
            return InteractionContext.CreateForPlayer(this, targetObject);
        }

        return _interactionContextFactory.Create(targetObject);
    }
}
