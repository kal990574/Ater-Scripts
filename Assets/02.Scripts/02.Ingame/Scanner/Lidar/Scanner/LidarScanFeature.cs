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
    
    private GameEventPublisher _eventPublisher;
    private LidarEffect _lidarEffect;
    private LidarRaycast _lidarRay;
    private float _energy = 1f;

    private AudioSource _soundInstance;

    public ScannableObject CurrentTarget { get; private set; }
    public LidarEffect LidarEffect => _lidarEffect;
    public LidarRaycast LidarRay => _lidarRay;
    public LidarScanConfigSO Config => _config;
    public Vector3 StartPos => _rayOrigin.position + _originOffset;
    public Transform Muzzle => muzzle;
    public LineRenderer LineRenderer => _lineRenderer;
    public ISoundService SoundService => SoundManager.Instance;
    
    public bool IsOnScan { get; private set; }
    public float Energy => _energy;
    public bool HasEnergy => _energy > 0f;
    
    public event Action<float> OnEnergyChanged;
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
        
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        _lidarRay = new(this);
        _lidarEffect = new(this);

        _energy = 1f;
    }

    public void ActiveScan()
    {
        if (!HasEnergy || IsOnScan) return;
        IsOnScan = true;

        
        _soundInstance = SoundService.PlayLoopSFX(_config.ScanActived,transform.position);
        
        _eventPublisher.TryPublish(
            context => new LidarScanStartedRawEvent(context));
    }

    public void UpdateScan(float deltaTime)
    {
        if (!IsOnScan)
        {
            return;
        }
        
        _lidarRay.Scan();

        ScannableObject previousTarget = CurrentTarget;
        ScannableObject newTarget = ResolveTarget(_lidarRay.HitMap, StartPos, transform.forward);

        HandleTargetChanged(previousTarget, newTarget);
        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            if (previousTarget == null)
            {
                CurrentTarget.OnScanStarted();
            }
            
            CurrentTarget.OnScanning(deltaTime);
        }

        if (_soundInstance != null)
        {
            _soundInstance.transform.position = transform.position;
        }

        LidarEffect.DrawLidarEffect(_lidarRay.RayResults, CurrentTarget);
    }

    private void HandleTargetChanged(ScannableObject previous, ScannableObject current)
    {
        if (previous == current)
        {
            return;
        }

        _eventPublisher.TryPublish(
            context => new LidarScanTargetChangedRawEvent(context, previous, current));

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
        
        
        SoundService.StopLoopSFX(_soundInstance);
        _soundInstance = null;
        
        _eventPublisher.TryPublish(
            context => new LidarScanStoppedRawEvent(context));
    }

    public void UpdateEnergy(float deltaTime, bool isHolding)
    {
        float previous = _energy;
        if (IsOnScan)
        {
            _energy -= _config.DrainRate * deltaTime;
            if (_energy <= 0f)
            {
                _energy = 0f;
                if (previous > 0f)
                {
                    _eventPublisher.TryPublish(
                        context => new LidarScanEnergyDepletedRawEvent(context));

                    SoundService.PlaySFX2D(_config.ScanFailed);
                }

                StopScan();
            }
        }
        else if (!isHolding)
        {
            _energy = Mathf.Min(1f, _energy + _config.RecoveryRate * deltaTime);
        }

        if (!Mathf.Approximately(previous, _energy))
        {
            OnEnergyChanged?.Invoke(_energy);
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
