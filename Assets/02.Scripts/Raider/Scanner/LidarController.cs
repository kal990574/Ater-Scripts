using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LidarController : MonoBehaviour
{
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Transform _shootPoint;
    [FormerlySerializedAs("_setting")] [SerializeField] private LidarConfig config;
    [SerializeField] private Vector3 _originOffset = Vector3.zero;
    
    private readonly Dictionary<Type, LidarAbility> _abilities = new();
    
    public LidarTarget CurrentTarget { get; private set; }

    public float RayDistance => config.RayDistance;
    public float ConeAngle => config.ConeAngle;
    public int RingCount => config.RingCount;
    public int RaysPerRing => config.RaysPerRing;
    public LayerMask HitMask => config.HitMask;
    
    public Vector3 StartPos => _rayOrigin.position + _originOffset;
    public Transform ShootPoint => _shootPoint;

    //현재는 타겟을 스캔할때 나타남
    //추후에는 호버중일때 80%투명도로 스캔 중일때 100%로 띄우기
    public event Action<LidarTarget> OnTargetFind;
    public event Action OnTargetLost;

    public bool IsOnScan { get; private set; } = false;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float deltaTime = Time.deltaTime;
            UpdateScan(deltaTime);
            IsOnScan = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopScan();
            IsOnScan = false;
        }
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
        // Case 1: 새로 타겟을 찾은 경우
        if (previous == null && current != null)
        {
            OnTargetFind?.Invoke(current);
            return;
        }

        // Case 2: 타겟이 바뀐 경우
        if (previous != null && current != null && previous != current)
        {
            OnTargetLost?.Invoke();
            previous.OnScanLost();
            OnTargetFind?.Invoke(current);
            return;
        }

        // Case 3: 타겟을 잃은 경우
        if (previous != null && current == null)
        {
            OnTargetLost?.Invoke();
            previous.OnScanLost();
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

                if (Mathf.Approximately(centerScore, bestCenterScore) == true && data.ClosestDistance < bestClosestDistance)
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
