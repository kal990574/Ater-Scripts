using System.Collections.Generic;
using UnityEngine;

public class LidarRaycast
{
    private readonly Dictionary<ScannableObject, TargetHitData> _hitMap = new();
    private readonly List<LidarRayData> _rayResults = new();
    private readonly List<LidarSurfaceHitSample> _surfaceHitSamples = new();
    private readonly LidarScanFeature _scanFeature;
    private readonly LidarScanConfigSO _config;
    private readonly IRaycastService _raycastService;

    public IReadOnlyDictionary<ScannableObject, TargetHitData> HitMap => _hitMap;
    public IReadOnlyList<LidarRayData> RayResults => _rayResults;
    public IReadOnlyList<LidarSurfaceHitSample> SurfaceHitSamples => _surfaceHitSamples;

    public LidarRaycast(LidarScanFeature scanFeature, IRaycastService raycastService = null)
    {
        _scanFeature = scanFeature;
        _config = scanFeature.Config;
        _raycastService = raycastService ?? new RaycastService();
    }

    public void Scan()
    {
        if (_scanFeature == null)
        {
            return;
        }

        ClearScanResults();
        Vector3 origin = _scanFeature.StartPos;

        foreach (Vector3 direction in EnumerateRayDirections())
        {
            CastRay(origin, direction);
        }
    }

    public void ClearScanResults()
    {
        _hitMap.Clear();
        _rayResults.Clear();
        _surfaceHitSamples.Clear();
    }

    public IEnumerable<Vector3> EnumerateRayDirections()
    {
        Quaternion rotation = _scanFeature.transform.rotation;

        yield return rotation * Vector3.forward;

        for (int ringIndex = 1; ringIndex <= _config.RingCount; ringIndex++)
        {
            float ringT = (float)ringIndex / _config.RingCount;
            float currentAngle = _config.ConeAngle * ringT;

            for (int rayIndex = 0; rayIndex < _config.RaysPerRing; rayIndex++)
            {
                float yaw = (360.0f / _config.RaysPerRing) * rayIndex;
                Vector3 direction = GetConeDirection(rotation, currentAngle, yaw);
                yield return direction;
            }
        }
    }

    public Vector3 GetConeDirection(Quaternion baseRotation, float angleFromForward, float yawAroundForward)
    {
        Vector3 localDirection = Quaternion.Euler(angleFromForward, 0.0f, 0.0f) * Vector3.forward;
        localDirection = Quaternion.AngleAxis(yawAroundForward, Vector3.forward) * localDirection;

        Vector3 worldDirection = baseRotation * localDirection;
        return worldDirection.normalized;
    }

    private void CastRay(Vector3 origin, Vector3 direction)
    {
        RaycastRequest request = _config.Query.CreateRequest(origin, direction);
        RaycastResult hit = _raycastService.Cast(request);

        if (hit.Hit == false)
        {
            HandleMiss(origin, direction);
            return;
        }

        ScannableObject target = hit.Collider.GetComponentInParent<ScannableObject>();

        if (IsInvalidTarget(target))
        {
            HandleBlockedHit(direction, hit);
            return;
        }

        HandleValidTargetHit(origin, direction, hit, target);
    }

    private static bool IsInvalidTarget(ScannableObject target)
    {
        if (target == null)
        {
            return true;
        }

        return target.IsProgressComplete;
    }

    private void HandleMiss(Vector3 origin, Vector3 direction)
    {
        float rayDistance = _config.Query.Distance;
        _rayResults.Add(
            new LidarRayData(
                direction,
                false,
                origin + direction * rayDistance,
                rayDistance,
                false));
    }

    private void HandleBlockedHit(Vector3 direction, RaycastResult hit)
    {
        _rayResults.Add(
            new LidarRayData(
                direction,
                true,
                hit.Point,
                hit.Distance,
                false));
    }

    private void HandleValidTargetHit(Vector3 origin, Vector3 direction, RaycastResult hit, ScannableObject target)
    {
        _rayResults.Add(
            new LidarRayData(
                direction,
                true,
                hit.Point,
                hit.Distance,
                true));

        _surfaceHitSamples.Add(new LidarSurfaceHitSample(target, hit.Point, hit.Normal, hit.Distance));
        UpdateTargetHitData(origin, hit, target);
    }

    private void UpdateTargetHitData(Vector3 origin, RaycastResult hit, ScannableObject target)
    {
        float distance = Vector3.Distance(origin, hit.Point);

        if (_hitMap.TryGetValue(target, out TargetHitData data))
        {
            data.HitCount++;

            if (distance < data.ClosestDistance)
            {
                data.ClosestDistance = distance;
                data.RepresentativePoint = hit.Point;
                data.RepresentativeNormal = hit.Normal;
            }

            _hitMap[target] = data;
            return;
        }

        TargetHitData newData = new(1, hit.Point, hit.Normal, distance);
        _hitMap.Add(target, newData);
    }
}
