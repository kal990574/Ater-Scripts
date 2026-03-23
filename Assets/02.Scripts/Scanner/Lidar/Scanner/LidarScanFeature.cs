using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class LidarScanFeature : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LidarConfig _config;
    
    [SerializeField] private LidarEffect _lidarEffect;
    [SerializeField] private LidarRaycast _lidarRay;

    [Header("Optional Settings")]
    [SerializeField] private Vector3 _originOffset = Vector3.zero;
    

    public LidarTarget CurrentTarget { get; private set; }
    public Vector3 StartPos => _rayOrigin.position + _originOffset;
    public Transform ShootPoint => _shootPoint;
    public bool IsOnScan { get; private set; }
    public float RayDistance => _config.RayDistance;
    public float ConeAngle => _config.ConeAngle;
    public int RingCount => _config.RingCount;
    public int RaysPerRing => _config.RaysPerRing;
    public LayerMask HitMask => _config.HitMask;

    public event Action<LidarTarget> OnTargetFind;
    public event Action OnTargetLost;
    

    public void Initialize()
    {
        if (_rayOrigin == null || _shootPoint == null || _config == null)
        {
            Debug.LogError($"[{nameof(LidarScanFeature)}] Required references are missing.", this);
            enabled = false;
        }

        if (TryGetComponent(out LidarEffect lidarEffect))
        {
            _lidarEffect = lidarEffect;
            _lidarEffect.Init(this);
        }
        else
        {
            Debug.LogError($"[Lidar] Missing Reference : LidarEffect");
        }
        
        if (TryGetComponent(out LidarRaycast lidarRay))
        {
            _lidarRay = lidarRay;
            _lidarRay.Init(this);
        }
        else
        {
            Debug.LogError($"[Lidar] Missing Reference : LidarRaycast");
        }
    }
    
    public void StopScan()
    {
        if (CurrentTarget != null)
        {
            OnTargetLost?.Invoke();
            CurrentTarget.OnScanLost();
            CurrentTarget = null;
        }

        _lidarRay.ClearScanResults();
        _lidarEffect.ResetLine();
        IsOnScan = false;
    }

    //이것도 여기있으면 안됨. 
    public void SubmitCurrentQte()
    {
        if (QTEManager.Instance == null)
        {
            return;
        }

        QTEManager.Instance.SubmitCurrent();
    }

    public void UpdateScan(float deltaTime)
    {
        if (!IsOnScan)
        {
            IsOnScan = true;
        }
        
        _lidarRay.Scan();

        LidarTarget previousTarget = CurrentTarget;
        LidarTarget newTarget = ResolveTarget(_lidarRay.HitMap, StartPos, transform.forward);

        HandleTargetChanged(previousTarget, newTarget);
        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            CurrentTarget.OnScanning(deltaTime);
        }
        
        _lidarEffect.DrawLidarEffect(_lidarRay.RayResults,CurrentTarget);
    }

    private void HandleTargetChanged(LidarTarget previous, LidarTarget current)
    {
        if (previous == null && current != null)
        {
            OnTargetFind?.Invoke(current);
            return;
        }

        if (previous != null && current != null && previous != current)
        {
            OnTargetLost?.Invoke();
            previous.OnScanLost();
            OnTargetFind?.Invoke(current);
            return;
        }

        if (previous != null && current == null)
        {
            OnTargetLost?.Invoke();
            previous.OnScanLost();
        }
    }

    private LidarTarget ResolveTarget(IReadOnlyDictionary<LidarTarget, TargetHitData> hitMap, Vector3 origin, Vector3 forward)
    {
        LidarTarget bestTarget = null;
        bool hasBest = false;

        int bestHitCount = int.MinValue;
        float bestCenterScore = float.MinValue;
        float bestClosestDistance = float.MaxValue;

        foreach (KeyValuePair<LidarTarget, TargetHitData> pair in hitMap)
        {
            LidarTarget candidate = pair.Key;
            TargetHitData data = pair.Value;

            Vector3 toRepresentativePoint = (data.RepresentativePoint - origin).normalized;
            float centerScore = Vector3.Dot(forward, toRepresentativePoint);

            if (hasBest == false)
            {
                bestTarget = candidate;
                bestHitCount = data.HitCount;
                bestCenterScore = centerScore;
                bestClosestDistance = data.ClosestDistance;
                hasBest = true;
                continue;
            }

            if (data.HitCount > bestHitCount)
            {
                bestTarget = candidate;
                bestHitCount = data.HitCount;
                bestCenterScore = centerScore;
                bestClosestDistance = data.ClosestDistance;
                continue;
            }

            if (data.HitCount == bestHitCount)
            {
                if (centerScore > bestCenterScore)
                {
                    bestTarget = candidate;
                    bestHitCount = data.HitCount;
                    bestCenterScore = centerScore;
                    bestClosestDistance = data.ClosestDistance;
                    continue;
                }

                if (Mathf.Approximately(centerScore, bestCenterScore) && data.ClosestDistance < bestClosestDistance)
                {
                    bestTarget = candidate;
                    bestHitCount = data.HitCount;
                    bestCenterScore = centerScore;
                    bestClosestDistance = data.ClosestDistance;
                }
            }
        }

        return bestTarget;
    }
}
