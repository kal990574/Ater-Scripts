using _02.Scripts.Player;
using System;
using UnityEngine;

//인풋을 통해 상호작용
//호버, 얻기, 사용하기, 놓기
public class PlayerInteractAbility : PlayerAbility
{
    private IPlayerInput _input;
    
    private void Start()
    {
        _input = _owner.Input;
    }
    
    private void Update()
    {
        
    }
}
