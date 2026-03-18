using System.Collections.Generic;
using UnityEngine;

public class LidarRaycastAbility : LidarAbility
{
    //타겟의 히트데이터 저장(히트된 횟수, 충돌거리등)
    private readonly Dictionary<LidarTarget, TargetHitData> _hitMap = new ();
    //타겟별 히트된 레이케스트 저장
    private readonly Dictionary<LidarTarget, List<RaycastHit>> _targetHitListMap = new ();
    //프레임별 레이케스트 정보 저장(연출에 추가)
    private readonly List<LidarRayData> _rayResults = new ();

    public IReadOnlyDictionary<LidarTarget, TargetHitData> HitMap => _hitMap;
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

    public bool TryGetTargetHits(LidarTarget target, out List<RaycastHit> hitList)
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
        //레이케스팅 및 히트 판정
        bool isHit = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            _controller.RayDistance,
            _controller.HitMask,
            QueryTriggerInteraction.Ignore);

        if (isHit == false)
        {
            //히트 되지 않았다면 길이는 최종까지 뻗어나간 결과
            _rayResults.Add(new LidarRayData(direction, false, origin + direction * _controller.RayDistance, _controller.RayDistance,false));
            return;
        }
        
        
        
        //장애물이든 타겟이든 히트 된 상황
        LidarTarget target = hit.collider.GetComponentInParent<LidarTarget>();
        if (target == null || target.IsProgressComplete)
        {
            //타겟이 없거나 이미 완료된 타겟이라면 벽에 가로막힌 판정
            _rayResults.Add(new LidarRayData(direction, true, hit.point, hit.distance,false));
            return;
        }
        
        //히트된 표면과 거리
        _rayResults.Add(new LidarRayData(direction, true, hit.point, hit.distance,true));
        //유효한 타겟이 있다면 타겟과 히트맵에 추가
        AddHitToTargetList(target, hit);
        float distance = Vector3.Distance(origin, hit.point);
        if (_hitMap.TryGetValue(target, out TargetHitData data))
        {
            data.HitCount++;

            if (distance < data.ClosestDistance)
            {
                data.ClosestDistance = distance;
                data.RepresentativePoint = hit.point;
            }

            _hitMap[target] = data;
            return;
        }
        TargetHitData newData = new TargetHitData(1, hit.point, distance);
        _hitMap.Add(target, newData);
    }

    private void AddHitToTargetList(LidarTarget target, RaycastHit hit)
    {
        if (_targetHitListMap.TryGetValue(target, out List<RaycastHit> hitList) == false)
        {
            hitList = new List<RaycastHit>();
            _targetHitListMap.Add(target, hitList);
        }

        hitList.Add(hit);
    }
}
