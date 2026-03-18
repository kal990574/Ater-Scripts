using UnityEngine;

public struct LidarRayData
{
    public Vector3 Direction;
    public bool IsHit;
    public Vector3 EndPoint;
    public float Distance;
    public bool HitTarget;

    public LidarRayData(Vector3 direction, bool isHit, Vector3 endPoint, float distance, bool hitTarget)
    {
        Direction = direction.normalized;
        IsHit = isHit;
        EndPoint = endPoint;
        Distance = distance;
        HitTarget = hitTarget;
    }
}
