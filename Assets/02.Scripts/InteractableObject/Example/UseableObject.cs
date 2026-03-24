using System;
using UnityEngine;
using UnityEngine.Events;

public class UseableObject : InteractableObject
{

    public override void Interact()
    {
        Debug.Log($"{gameObject.name}을 사용함");
        OnInteractActivate();
    }
}
