using UnityEngine;

public readonly struct RaycastResult
{
    public bool Hit { get; }
    public Collider Collider { get; }
    public Vector3 Point { get; }
    public Vector3 Normal { get; }
    public float Distance { get; }

    public RaycastResult(bool hit, Collider collider, Vector3 point, Vector3 normal, float distance)
    {
        Hit = hit;
        Collider = collider;
        Point = point;
        Normal = normal;
        Distance = distance;
    }

    public static RaycastResult Miss => new(false, null, Vector3.zero, Vector3.zero, 0f);
}
