using UnityEngine;

[CreateAssetMenu(
    fileName = "SoundJumpScareDefinition",
    menuName = "Ater/JumpScare/Sub JumpScare/Sound Definition")]
public class SoundJumpScareDefinitionSO : SubJumpScareDefinitionSO
{
    [Header("Weak")]
    [SerializeField] private float weakRadiusMin = 10.0f;
    [SerializeField] private float weakRadiusMax = 16.0f;
    [SerializeField] private float weakTensionDecrease = 3.0f;

    [Header("Medium")]
    [SerializeField] private float mediumRadiusMin = 6.0f;
    [SerializeField] private float mediumRadiusMax = 12.0f;
    [SerializeField] private float mediumTensionDecrease = 5.0f;

    [Header("Strong")]
    [SerializeField] private float strongRadiusMin = 2.0f;
    [SerializeField] private float strongRadiusMax = 7.0f;
    [SerializeField] private float strongTensionDecrease = 7.0f;

    public float GetRadiusMin(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakRadiusMin;

            case EJumpScareIntensity.Medium:
                return mediumRadiusMin;

            case EJumpScareIntensity.Strong:
                return strongRadiusMin;
        }

        return weakRadiusMin;
    }

    public float GetRadiusMax(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakRadiusMax;

            case EJumpScareIntensity.Medium:
                return mediumRadiusMax;

            case EJumpScareIntensity.Strong:
                return strongRadiusMax;
        }

        return weakRadiusMax;
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