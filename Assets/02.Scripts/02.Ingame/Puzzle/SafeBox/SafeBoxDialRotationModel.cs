using UnityEngine;

public class SafeBoxDialRotationModel
{
    private const int MinValue = 0;
    private const int MaxValue = 15;
    private const int ValueCount = 16;

    private readonly float _degreesPerStep;

    public SafeBoxDialRotationModel(float degreesPerStep)
    {
        _degreesPerStep = degreesPerStep;
    }

    public SafeBoxDialRotationStep GetStep(float visualAngle)
    {
        float stepIndex = Mathf.Round(-visualAngle / _degreesPerStep);
        float normalizedStepIndex = Mod(stepIndex, ValueCount);
        int value = Mathf.Clamp((int)normalizedStepIndex, MinValue, MaxValue);
        float snappedAngle = -value * _degreesPerStep;
        return new SafeBoxDialRotationStep(value, snappedAngle);
    }

    private static float Mod(float value, float modulus)
    {
        float result = value % modulus;
        if (result < 0f)
        {
            result += modulus;
        }

        return result;
    }
}
