using System;
using UnityEngine;

public class LidarAbility : MonoBehaviour
{
    protected LidarController _controller;

    protected void Awake()
    {
        _controller = GetComponentInParent<LidarController>();
    }
}
