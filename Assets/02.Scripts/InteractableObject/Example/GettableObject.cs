using System;
using UnityEngine;

//인벤토리에 넣을 수 있는 오브젝트
public class GettableObject : InteractableObject
{
    [SerializeField] private int _itemId;
    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : 현재 상호작용 가능한 상태가 아님");
            return;
        }
        
        InventoryManager.Instance.TryAddItem(_itemId);
        OnInteractActivate();
        gameObject.SetActive(false);
    }
}
