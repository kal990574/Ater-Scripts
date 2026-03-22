using System;
using UnityEngine;

[Serializable]
public class LidarConfig
{
    [Header("Ray")]
    [Min(0.01f)] public float RayDistance = 10.0f;
    [Range(0f, 180f)] public float ConeAngle = 45.0f;

    [Header("Density")]
    [Min(1)] public int RingCount = 4;
    [Min(1)] public int RaysPerRing = 12;

    [Header("Mask")]
    public LayerMask HitMask = ~0;
}
