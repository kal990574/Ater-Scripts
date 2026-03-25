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
    public WorldItemController Target => GetAbility<PlayerDetectAbility>().CurrentTarget;

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
        if (Target != null && _input.InteractInput)
        {
            //타게팅된 오브젝트와 상호작용
            GetAbility<PlayerInteractAbility>().Interact(Target);
        }
        
        //스캔모드일 경우 스캔 인풋
        if (InteractMode == PlayerInteractMode.Scan)
        {
            ScanModeInput();
        }
        
        if (_input.ScannerToggleInput)
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
        
        if (_interactMode == PlayerInteractMode.Item && _input.LmbPressInput)
        {
            GetAbility<PlayerInventoryAbility>().TryThrowItem();
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
