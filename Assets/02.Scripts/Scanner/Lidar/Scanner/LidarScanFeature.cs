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
    
    //캐싱
    private LidarEffect _lidarEffect;
    private LidarRaycast _lidarRay;
    
    public ScannableObject CurrentTarget { get; private set; }
    
    //프로퍼티
    public LidarEffect LidarEffect => _lidarEffect;
    public LidarRaycast LidarRay => _lidarRay;
    public LidarScanConfigSO Config => _config;
    public Vector3 StartPos => _rayOrigin.position + _originOffset; //레이가 시작하는 위치
    public Transform Muzzle => muzzle;                              //총구위치
    public LineRenderer LineRenderer => _lineRenderer;              //라인 렌더러
    public bool IsOnScan { get; private set; }

    //이벤트
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
        Debug.Log($"LineRenderer Object: {_lineRenderer.gameObject.name}", _lineRenderer);
        Debug.Log($"Instance ID: {_lineRenderer.GetInstanceID()}", _lineRenderer);
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
            CurrentTarget.OnScanLost();
            CurrentTarget = null;
        }

        _lidarRay.ClearScanResults();
        LidarEffect.ResetLine();
        IsOnScan = false;
    }

    //이것도 여기있으면 안됨. 나중에 수정할것
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

        ScannableObject previousTarget = CurrentTarget;
        ScannableObject newTarget = ResolveTarget(_lidarRay.HitMap, StartPos, transform.forward);

        HandleTargetChanged(previousTarget, newTarget);
        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            CurrentTarget.OnScanning(deltaTime);
        }
        
        LidarEffect.DrawLidarEffect(_lidarRay.RayResults,CurrentTarget);
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
