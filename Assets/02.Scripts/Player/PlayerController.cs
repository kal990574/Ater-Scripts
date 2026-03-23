using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInteractMode
{
    Item,
    Scan,
}
//플레이어 생명주기 담당
//각종 설정 및 콘피그는 여기에 모두 할당하고 하위 어빌리티로 전달할것
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfigSO playerConfig;
    
    
    [SerializeField]private bool _canMove = true;
    [SerializeField]private bool _canRotate= true;
    [SerializeField]private PlayerInteractMode _interactMode= PlayerInteractMode.Scan;
    
    //Cache
    private Dictionary<Type, PlayerAbility> _abilities;
    private IPlayerInput _input;
    
    

    public PlayerConfigSO Config => playerConfig;
    public IPlayerInput Input => _input;
    public bool CanMove => _canMove;
    public bool CanRotate => _canRotate;
    public PlayerInteractMode InteractMode => _interactMode;

    //초기화 로직
    private void Awake()
    {
        if (_input == null)
        {
            _input = GetComponentInChildren<IPlayerInput>();
        }
        
    }

    //파괴시 로직
    private void OnDestroy()
    {
        
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

        throw new Exception($"[PlayerController] Ability {type.Name} not found on {gameObject.name}.");
    }
}
