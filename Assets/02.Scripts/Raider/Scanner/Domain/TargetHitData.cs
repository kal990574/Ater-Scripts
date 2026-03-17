using System;
using UnityEngine;

public struct TargetHitData
{
    public int HitCount;                    //히트한 레이의 수
    public Vector3 RepresentativePoint;     //대표 히트 위치(가장 중앙과 가까운)
    public float ClosestDistance;           //시작 위치 기준 거리

    public TargetHitData(int hitCount, Vector3 representativePoint, float closestDistance)
    {
        if (hitCount < 0) throw new Exception("hitCount는 음수가 될 수 없다.");
        if (closestDistance < 0f) throw new Exception("closestDistance는 음수가 될 수 없다.");
        HitCount = hitCount;
        RepresentativePoint = representativePoint;
        ClosestDistance = closestDistance;
    }
}