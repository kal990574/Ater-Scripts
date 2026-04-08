using UnityEngine;

public readonly struct SonarScanStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public Vector3 Origin { get; }
    public Vector3 Direction { get; }
    public float ExpandSpeed { get; }
    public float ScanRadius { get; }
    public float ScanAngle { get; }

    public SonarScanStartedRawEvent(
        GameEventContext context,
        Vector3 origin,
        Vector3 direction,
        float expandSpeed,
        float scanRadius,
        float scanAngle)
    {
        Context = context;
        Origin = origin;
        Direction = direction;
        ExpandSpeed = expandSpeed;
        ScanRadius = scanRadius;
        ScanAngle = scanAngle;
    }
}