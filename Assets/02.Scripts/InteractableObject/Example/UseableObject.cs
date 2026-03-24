using System;
using UnityEngine;
using UnityEngine.Events;

public class UseableObject : InteractableObject
{

    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : 현재 상호작용 가능한 상태가 아님");
            return;
        }
        
        Debug.Log($"{gameObject.name}을 사용함");
        OnInteractActivate();

        _isInteractActive = false;
    }
}
