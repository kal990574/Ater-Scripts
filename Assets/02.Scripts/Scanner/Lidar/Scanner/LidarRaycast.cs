using System.Collections.Generic;
using UnityEngine;

public class LidarRaycast
{
    private readonly Dictionary<LidarTarget, TargetHitData> _hitMap = new();
    private readonly List<LidarRayData> _rayResults = new();
    private readonly LidarScanFeature _scanFeature;
    private readonly LidarScanConfigSO _config;
    
    public IReadOnlyDictionary<LidarTarget, TargetHitData> HitMap => _hitMap;
    public IReadOnlyList<LidarRayData> RayResults => _rayResults;

    public LidarRaycast(LidarScanFeature scanFeature)
    {
        _scanFeature = scanFeature;
        _config =  scanFeature.Config;
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
    }

    //주어진 설정으로 레이의 발사 방향 저장
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
    
    //원뿔 모양 형성, 단 레이 디버그에도 사용됨
    public Vector3 GetConeDirection(Quaternion baseRotation, float angleFromForward, float yawAroundForward)
    {
        Vector3 localDirection = Quaternion.Euler(angleFromForward, 0.0f, 0.0f) * Vector3.forward;
        localDirection = Quaternion.AngleAxis(yawAroundForward, Vector3.forward) * localDirection;

        Vector3 worldDirection = baseRotation * localDirection;
        return worldDirection.normalized;
    }

    //레이를 생성
    private void CastRay(Vector3 origin, Vector3 direction)
    {
        bool isHit = Physics.Raycast(origin, direction, out RaycastHit hit,
            _config.RayDistance, _config.HitMask, QueryTriggerInteraction.Ignore);

        if (isHit == false)
        {
            //끝까지 나간 레이 처리
            HandleMiss(origin, direction);
            return;
        }

        LidarTarget target = hit.collider.GetComponentInParent<LidarTarget>();

        if (IsInvalidTarget(target))
        {
            //벽에 막힌 레이 처리
            HandleBlockedHit(direction, hit);
            return;
        }
        
        //타겟을 감지한 레이 처리
        HandleValidTargetHit(origin, direction, hit, target);
    }

    //LidarTarget이면서 추상화가 되어있는 것
    private bool IsInvalidTarget(LidarTarget target)
    {
        if (target == null)
        {
            return true;
        }

        if (target.IsProgressComplete)
        {
            return true;
        }

        return false;
    }

    private void HandleMiss(Vector3 origin, Vector3 direction)
    {
        _rayResults.Add(
            new LidarRayData(
                direction,
                false,
                origin + direction * _config.RayDistance,
                _config.RayDistance,
                false));
    }

    private void HandleBlockedHit(Vector3 direction, RaycastHit hit)
    {
        _rayResults.Add(
            new LidarRayData(
                direction,
                true,
                hit.point,
                hit.distance,
                false));
    }

    private void HandleValidTargetHit(Vector3 origin, Vector3 direction, RaycastHit hit, LidarTarget target)
    {
        _rayResults.Add(
            new LidarRayData(
                direction,
                true,
                hit.point,
                hit.distance,
                true));
        
        UpdateTargetHitData(origin, hit, target);
    }

    private void UpdateTargetHitData(Vector3 origin, RaycastHit hit, LidarTarget target)
    {
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
}