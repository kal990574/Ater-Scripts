using UnityEngine;

[CreateAssetMenu(
    fileName = "PostProcessJumpScareDefinition",
    menuName = "Ater/JumpScare/Sub JumpScare/Post Process Definition")]
public class PostProcessJumpScareDefinitionSO : SubJumpScareDefinitionSO
{
    [Header("Weak")]
    [SerializeField] private float weakDurationMin = 2.5f;
    [SerializeField] private float weakDurationMax = 4.0f;
    [SerializeField] private float weakTensionDecrease = 3.0f;

    [Header("Medium")]
    [SerializeField] private float mediumDurationMin = 5.0f;
    [SerializeField] private float mediumDurationMax = 8.0f;
    [SerializeField] private float mediumTensionDecrease = 5.0f;

    [Header("Strong")]
    [SerializeField] private float strongDurationMin = 12.0f;
    [SerializeField] private float strongDurationMax = 16.0f;
    [SerializeField] private float strongTensionDecrease = 7.0f;

    public float GetDurationMin(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakDurationMin;

            case EJumpScareIntensity.Medium:
                return mediumDurationMin;

            case EJumpScareIntensity.Strong:
                return strongDurationMin;
        }

        return weakDurationMin;
    }

    public float GetDurationMax(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakDurationMax;

            case EJumpScareIntensity.Medium:
                return mediumDurationMax;

            case EJumpScareIntensity.Strong:
                return strongDurationMax;
        }

        return weakDurationMax;
    }

    public float GetTensionDecrease(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakTensionDecrease;

            case EJumpScareIntensity.Medium:
                return mediumTensionDecrease;

            case EJumpScareIntensity.Strong:
                return strongTensionDecrease;
        }

        return 0.0f;
    }
}