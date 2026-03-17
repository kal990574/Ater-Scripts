using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LidarSetting
{
    public float RayDistance = 10.0f;
    public float ConeAngle = 45.0f;
    public int RingCount = 4;
    public int RaysPerRing = 12;
    public LayerMask HitMask = ~0;
}