using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "LidarConfig", menuName = "Ater/Scanner/LidarConfig")]
public class LidarScanConfigSO : ScriptableObject
{
    [Header("Ray")]
    [SerializeField] private RaycastSetting _query = new(10.0f, ~0, QueryTriggerInteraction.Ignore);
    [SerializeField] [Range(0f, 180f)] private float _coneAngle = 45.0f;

    [Header("Density")]
    [SerializeField] [Min(1)] private int _ringCount = 4;
    [SerializeField] [Min(1)] private int _raysPerRing = 12;

    [Header("Effect")]
    [Min(0f)]
    [SerializeField] private float _drawDelay = 0.1f;


    public RaycastSetting Query => _query;
    public float ConeAngle => _coneAngle;
    public int RingCount => _ringCount;
    public int RaysPerRing => _raysPerRing;
    public float DrawDelay => _drawDelay;
}
