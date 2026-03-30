using System;
using System.Collections.Generic;
using UnityEngine;

public class LidarScanFeature : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private LidarScanConfigSO _config;
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Transform muzzle;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Optional Settings")]
    [SerializeField] private Vector3 _originOffset = Vector3.zero;

    private LidarEffect _lidarEffect;
    private LidarRaycast _lidarRay;

    public ScannableObject CurrentTarget { get; private set; }

    public LidarEffect LidarEffect => _lidarEffect;
    public LidarRaycast LidarRay => _lidarRay;
    public LidarScanConfigSO Config => _config;
    public Vector3 StartPos => _rayOrigin.position + _originOffset;
    public Transform Muzzle => muzzle;
    public LineRenderer LineRenderer => _lineRenderer;
    public bool IsOnScan { get; private set; }

    public event Action<ScannableObject> OnTargetFind;
    public event Action OnTargetLost;

    public void Initialize()
    {
        if (_config == null)
        {
            Debug.LogError($"[{nameof(LidarScanFeature)}] Lidar Config is Missing.", this);
            return;
        }

        if (_lineRenderer == null)
        {
            Debug.LogError($"[{nameof(LidarEffect)}] LineRenderer reference is missing.");
            return;
        }

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        _lidarRay = new(this);
        _lidarEffect = new(this);
    }

    public void StopScan()
    {
        if (CurrentTarget != null)
        {
            OnTargetLost?.Invoke();
            CurrentTarget.OnScanStopped();

            CurrentTarget = null;
        }

        _lidarRay.ClearScanResults();
        LidarEffect.ResetLine();
        IsOnScan = false;
    }

    public void UpdateScan(float deltaTime)
    {
        if (IsOnScan == false)
        {
            IsOnScan = true;
        }

        _lidarRay.Scan();

        ScannableObject previousTarget = CurrentTarget;
        ScannableObject newTarget = ResolveTarget(_lidarRay.HitMap, StartPos, transform.forward);

        HandleTargetChanged(previousTarget, newTarget);
        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            CurrentTarget.OnScanning(deltaTime);
        }

        LidarEffect.DrawLidarEffect(_lidarRay.RayResults, CurrentTarget);
    }

    private void HandleTargetChanged(ScannableObject previous, ScannableObject current)
    {
        if (previous == null && current != null)
        {
            OnTargetFind?.Invoke(current);
            return;
        }

        if (previous != null && current != null && previous != current)
        {
            OnTargetLost?.Invoke();
            previous.OnScanStopped();
            OnTargetFind?.Invoke(current);
            return;
        }

        if (previous != null && current == null)
        {
            OnTargetLost?.Invoke();
            previous.OnScanStopped();
        }
    }

    private ScannableObject ResolveTarget(IReadOnlyDictionary<ScannableObject, TargetHitData> hitMap, Vector3 origin, Vector3 forward)
    {
        ScannableObject bestTarget = null;
        bool hasBest = false;

        int bestHitCount = int.MinValue;
        float bestCenterScore = float.MinValue;
        float bestClosestDistance = float.MaxValue;

        foreach (KeyValuePair<ScannableObject, TargetHitData> pair in hitMap)
        {
            ScannableObject candidate = pair.Key;
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
