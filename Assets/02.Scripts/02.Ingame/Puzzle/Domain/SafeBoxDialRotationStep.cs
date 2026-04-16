public readonly struct SafeBoxDialRotationStep
{
    public SafeBoxDialRotationStep(int value, float visualAngle)
    {
        Value = value;
        VisualAngle = visualAngle;
    }

    public int Value { get; }
    public float VisualAngle { get; }
}
