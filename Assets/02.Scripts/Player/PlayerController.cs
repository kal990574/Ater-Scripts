using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInteractMode
{
    Item,
    Scan,
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

    public PlayerConfigSO Config => _playerConfig;
    public IPlayerInput Input => _input;
    public bool CanMove => _canMove;
    public bool CanRotate => _canRotate;
    public PlayerInteractMode InteractMode => _interactMode;
    public InteractController Target => GetAbility<PlayerDetectAbility>().CurrentTarget;

    public event Action<PlayerInteractMode> OnModeChanged;

    private void Awake()
    {
        if (_input == null)
        {
            _input = GetComponentInChildren<IPlayerInput>();
        }
    }

    private void Update()
    {
        if (Target != null)
        {
            ItemModeInput();
        }

        if (InteractMode == PlayerInteractMode.Scan)
        {
            ScanModeInput();
        }

        if (_input.ScannerToggleInput)
        {
            SetActionMode(PlayerInteractMode.Scan);
        }

        if (_input.InventoryToggleInput)
        {
            GetAbility<PlayerInventoryAbility>().ToggleInventory();
        }

        if (_input.ItemSlotInput >= 0 && _input.ItemSlotInput <= 5)
        {
            SetActionMode(PlayerInteractMode.Item);
        }
    }

    private void ItemModeInput()
    {
        if (_input.InteractInput)
        {
            GetAbility<PlayerInteractAbility>().Interact(Target);
        }
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
            scanAbility.LidarScanActiveAndUpdate();
        }

        if (_input.InteractInput)
        {
            scanAbility.LidarSubmitQTE();
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
        _interactMode = mode;
        OnModeChanged?.Invoke(mode);
    }
}
