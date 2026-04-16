using UnityEngine;

public readonly struct TransformInteractablePose
{
    public TransformInteractablePose(Vector3 position, Vector3 eulerAngles, bool useLocalSpace)
    {
        Position = position;
        EulerAngles = eulerAngles;
        UseLocalSpace = useLocalSpace;
    }

    public Vector3 Position { get; }
    public Vector3 EulerAngles { get; }
    public bool UseLocalSpace { get; }
}
