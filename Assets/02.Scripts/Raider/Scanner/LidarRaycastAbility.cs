using System.Collections.Generic;
using UnityEngine;

public class LidarRaycastAbility : LidarAbility
{
    private readonly Dictionary<LidarScannableObject, TargetHitData> _hitMap = new Dictionary<LidarScannableObject, TargetHitData>();
    private readonly Dictionary<LidarScannableObject, List<RaycastHit>> _targetHitListMap = new Dictionary<LidarScannableObject, List<RaycastHit>>();
    private readonly List<LidarRayData> _rayResults = new ();

    public IReadOnlyDictionary<LidarScannableObject, TargetHitData> HitMap => _hitMap;
    public IReadOnlyList<LidarRayData> RayResults => _rayResults;

    public void Scan()
    {
        ClearScanResults();

        Vector3 origin = _controller.StartPos;

        foreach (Vector3 direction in EnumerateRayDirections())
        {
            CastRay(origin, direction);
        }
    }

    public void ClearScanResults()
    {
        _hitMap.Clear();
        _targetHitListMap.Clear();
        _rayResults.Clear();
    }

    public IEnumerable<Vector3> EnumerateRayDirections()
    {
        Quaternion rotation = transform.rotation;

        yield return rotation * Vector3.forward;

        for (int ringIndex = 1; ringIndex <= _controller.RingCount; ringIndex++)
        {
            float ringT = (float)ringIndex / _controller.RingCount;
            float currentAngle = _controller.ConeAngle * ringT;

            for (int rayIndex = 0; rayIndex < _controller.RaysPerRing; rayIndex++)
            {
                float yaw = (360.0f / _controller.RaysPerRing) * rayIndex;
                Vector3 direction = GetConeDirection(rotation, currentAngle, yaw);
                yield return direction;
            }
        }
    }

    public IEnumerable<Vector3> EnumerateOutlineDirections(int segmentCount)
    {
        Quaternion rotation = transform.rotation;

        for (int segmentIndex = 0; segmentIndex <= segmentCount; segmentIndex++)
        {
            float yaw = (360.0f / segmentCount) * segmentIndex;
            Vector3 direction = GetConeDirection(rotation, _controller.ConeAngle, yaw);
            yield return direction;
        }
    }

    public bool TryGetTargetHits(LidarScannableObject target, out List<RaycastHit> hitList)
    {
        hitList = null;

        if (target == null)
        {
            return false;
        }

        if (_targetHitListMap.TryGetValue(target, out hitList) == false)
        {
            hitList = null;
            return false;
        }

        if (hitList == null || hitList.Count == 0)
        {
            hitList = null;
            return false;
        }

        return true;
    }

    private Vector3 GetConeDirection(Quaternion baseRotation, float angleFromForward, float yawAroundForward)
    {
        Vector3 localDirection = Quaternion.Euler(angleFromForward, 0.0f, 0.0f) * Vector3.forward;
        localDirection = Quaternion.AngleAxis(yawAroundForward, Vector3.forward) * localDirection;

        Vector3 worldDirection = baseRotation * localDirection;
        return worldDirection.normalized;
    }

    private void CastRay(Vector3 origin, Vector3 direction)
    {
        bool isHit = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            _controller.RayDistance,
            _controller.HitMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == false)
        {
            _rayResults.Add(new LidarRayData(direction, false, origin + direction * _controller.RayDistance, _controller.RayDistance));
            return;
        }

        _rayResults.Add(new LidarRayData(direction, true, hit.point, hit.distance));

        LidarScannableObject scannableObject = hit.collider.GetComponentInParent<LidarScannableObject>();

        if (scannableObject == null)
        {
            return;
        }

        AddHitToTargetList(scannableObject, hit);

        float distance = Vector3.Distance(origin, hit.point);

        if (_hitMap.TryGetValue(scannableObject, out TargetHitData data) == true)
        {
            data.HitCount++;

            if (distance < data.ClosestDistance)
            {
                data.ClosestDistance = distance;
                data.RepresentativePoint = hit.point;
            }

            _hitMap[scannableObject] = data;
            return;
        }

        TargetHitData newData = new TargetHitData(1, hit.point, distance);
        _hitMap.Add(scannableObject, newData);
    }

    private void AddHitToTargetList(LidarScannableObject scannableObject, RaycastHit hit)
    {
        if (_targetHitListMap.TryGetValue(scannableObject, out List<RaycastHit> hitList) == false)
        {
            hitList = new List<RaycastHit>();
            _targetHitListMap.Add(scannableObject, hitList);
        }

        hitList.Add(hit);
    }
}
