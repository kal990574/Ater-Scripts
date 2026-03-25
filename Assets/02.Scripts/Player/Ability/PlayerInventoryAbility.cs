using _02.Scripts.Player;
using System;
using UnityEngine;

//아이템 들기 , 놓기 및 던지기
public class PlayerInventoryAbility : PlayerAbility
{
    private IPlayerInput _input;
    private InventoryManager _inventoryManager;
    
    private void Start()
    {
        _inventoryManager = InventoryManager.Instance;
        _input = _owner.Input;
    }

    public void Init(InventoryManager inventoryManager)
    {
        _inventoryManager =  inventoryManager;
    }
    
    public void ToggleInventory()
    {
        if (_inventoryManager == null)
        {
            return;
        }
        
        if (_input.InventoryToggleInput)
        {
            _inventoryManager.ToggleInventory();
        }
    }
}
