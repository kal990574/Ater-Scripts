using UnityEngine;

public readonly struct LidarSurfaceHitSample
{
    public ScannableObject Target { get; }
    public Vector3 Point { get; }
    public Vector3 Normal { get; }
    public float Distance { get; }

    public LidarSurfaceHitSample(ScannableObject target, Vector3 point, Vector3 normal, float distance)
    {
        Target = target;
        Point = point;
        Normal = normal;
        Distance = distance;
    }
}
