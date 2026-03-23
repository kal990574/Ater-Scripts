using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LidarConfig", menuName = "Ater/Scanner/LidarConfig")]
public class LidarScanConfigSO : ScriptableObject
{
    [Header("Ray")]
    [SerializeField] [Min(0.01f)] private float _rayDistance = 10.0f;
    [SerializeField] [Range(0f, 180f)] private float _coneAngle = 45.0f;

    [Header("Density")]
    [SerializeField] [Min(1)] private int _ringCount = 4;
    [SerializeField] [Min(1)] private int _raysPerRing = 12;

    [Header("Mask")]
    [SerializeField] private LayerMask _hitMask = ~0;

    [Header("Effect")]
    [Min(0f)]
    [SerializeField] private float _drawDelay = 0.1f;
    
    public float RayDistance => _rayDistance;
    public float ConeAngle => _coneAngle;
    public int RingCount => _ringCount;
    public int RaysPerRing => _raysPerRing;
    public LayerMask HitMask => _hitMask;
    public float DrawDelay => _drawDelay;
}
