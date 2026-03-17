using System;
using System.Collections.Generic;
using UnityEngine;

public class LidarController : MonoBehaviour
{
    [SerializeField] private Transform _rayOrigin;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LidarSetting _setting;
    [SerializeField] private Vector3 _originOffset = Vector3.zero;
    
    private readonly Dictionary<Type, LidarAbility> _abilities = new Dictionary<Type, LidarAbility>();
    
    public LidarScannableObject CurrentTarget { get; private set; }

    public float RayDistance => _setting.RayDistance;
    public float ConeAngle => _setting.ConeAngle;
    public int RingCount => _setting.RingCount;
    public int RaysPerRing => _setting.RaysPerRing;
    public LayerMask HitMask => _setting.HitMask;
    public Vector3 StartPos => _rayOrigin.position + _originOffset;
    public Transform RayOrigin => _rayOrigin;
    public Transform ShootPoint => _shootPoint;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateScan();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetLidar();
        }
    }

    private void UpdateScan()
    {
        LidarRaycastAbility raycastAbility = GetAbility<LidarRaycastAbility>();
        raycastAbility.Scan();

        CurrentTarget = ResolveTarget(raycastAbility.HitMap, StartPos, transform.forward);

        GetAbility<LidarEffectAbility>().DrawLidarEffect(CurrentTarget);
    }

    public void ResetLidar()
    {
        CurrentTarget = null;
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

    private LidarScannableObject ResolveTarget(IReadOnlyDictionary<LidarScannableObject, TargetHitData> hitMap, Vector3 origin, Vector3 forward)
    {
        LidarScannableObject bestTarget = null;
        bool hasBest = false;

        int bestHitCount = int.MinValue;
        float bestCenterScore = float.MinValue;
        float bestClosestDistance = float.MaxValue;

        foreach (KeyValuePair<LidarScannableObject, TargetHitData> pair in hitMap)
        {
            LidarScannableObject candidate = pair.Key;
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
