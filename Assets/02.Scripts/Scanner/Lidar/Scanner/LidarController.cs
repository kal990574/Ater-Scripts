using _02.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class LidarController : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Transform _shootPoint;
    [FormerlySerializedAs("config")]
    [SerializeField] private LidarConfig _config;

    [Header("Optional Settings")]
    [SerializeField] private Vector3 _originOffset = Vector3.zero;

    private readonly Dictionary<Type, LidarAbility> _abilities = new();
    private IPlayerInput _input;

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

    private void Awake()
    {
        if (_rayOrigin == null || _shootPoint == null || _config == null)
        {
            Debug.LogError($"[{nameof(LidarController)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        _input = GetComponentInParent<IPlayerInput>();
        if (_input == null)
        {
            Debug.LogError($"[{nameof(LidarController)}] {nameof(IPlayerInput)} not found.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (_input.LmbPressInput)
        {
            UpdateScan(Time.deltaTime);
            IsOnScan = true;
        }

        if (_input.InteractInput)
        {
            SubmitCurrentQte();
        }

        if (_input.LmbReleaseInput)
        {
            StopScan();
            IsOnScan = false;
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

        GetAbility<LidarRaycastAbility>().ClearScanResults();
        GetAbility<LidarEffectAbility>().ResetLine();
    }

    public T GetAbility<T>() where T : LidarAbility
    {
        Type type = typeof(T);

        if (_abilities.TryGetValue(type, out LidarAbility ability))
        {
            return ability as T;
        }

        ability = GetComponentInChildren<T>();
        if (ability != null)
        {
            _abilities[type] = ability;
            return ability as T;
        }

        throw new Exception($"[LidarController] Ability {type.Name} not found on {gameObject.name}.");
    }

    private static void SubmitCurrentQte()
    {
        if (QTEManager.Instance == null)
        {
            return;
        }

        QTEManager.Instance.SubmitCurrent();
    }

    private void UpdateScan(float deltaTime)
    {
        LidarRaycastAbility raycastAbility = GetAbility<LidarRaycastAbility>();
        raycastAbility.Scan();

        LidarTarget previousTarget = CurrentTarget;
        LidarTarget newTarget = ResolveTarget(raycastAbility.HitMap, StartPos, transform.forward);

        HandleTargetChanged(previousTarget, newTarget);
        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            CurrentTarget.OnScanning(deltaTime);
        }

        GetAbility<LidarEffectAbility>().DrawLidarEffect(CurrentTarget);
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
