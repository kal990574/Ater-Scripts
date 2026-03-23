using System;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    protected PlayerController _owner;

    protected virtual void Awake()
    {
        _owner = GetComponentInParent<PlayerController>();
    }
}
