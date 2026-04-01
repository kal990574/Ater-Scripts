using UnityEngine;

[CreateAssetMenu(
    fileName = "FakeEnemyJumpScareDefinition",
    menuName = "Ater/JumpScare/Sub JumpScare/Fake Enemy Definition")]
public class FakeEnemyJumpScareDefinitionSO : SubJumpScareDefinitionSO
{
    [Header("Weak")]
    [SerializeField] private float weakSpawnDistanceMin = 12.0f;
    [SerializeField] private float weakSpawnDistanceMax = 18.0f;
    [SerializeField] private bool weakUseAnimation = false;
    [SerializeField] private float weakSuccessTensionDecrease = 5.0f;
    [SerializeField] private float weakFailureTensionIncrease = 10.0f;

    [Header("Medium")]
    [SerializeField] private float mediumSpawnDistanceMin = 8.0f;
    [SerializeField] private float mediumSpawnDistanceMax = 14.0f;
    [SerializeField] private bool mediumUseAnimation = false;
    [SerializeField] private float mediumSuccessTensionDecrease = 7.0f;
    [SerializeField] private float mediumFailureTensionIncrease = 14.0f;

    [Header("Strong")]
    [SerializeField] private float strongSpawnDistanceMin = 5.0f;
    [SerializeField] private float strongSpawnDistanceMax = 10.0f;
    [SerializeField] private bool strongUseAnimation = true;
    [SerializeField] private float strongSuccessTensionDecrease = 10.0f;
    [SerializeField] private float strongFailureTensionIncrease = 20.0f;

    [Header("Shared")]
    [SerializeField] private float detectionTimeoutSeconds = 6.0f;

    public float DetectionTimeoutSeconds => detectionTimeoutSeconds;

    public float GetSpawnDistanceMin(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakSpawnDistanceMin;

            case EJumpScareIntensity.Medium:
                return mediumSpawnDistanceMin;

            case EJumpScareIntensity.Strong:
                return strongSpawnDistanceMin;
        }

        return weakSpawnDistanceMin;
    }

    public float GetSpawnDistanceMax(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakSpawnDistanceMax;

            case EJumpScareIntensity.Medium:
                return mediumSpawnDistanceMax;

            case EJumpScareIntensity.Strong:
                return strongSpawnDistanceMax;
        }

        return weakSpawnDistanceMax;
    }

    public bool GetUseAnimation(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakUseAnimation;

            case EJumpScareIntensity.Medium:
                return mediumUseAnimation;

            case EJumpScareIntensity.Strong:
                return strongUseAnimation;
        }

        return false;
    }

    public float GetSuccessTensionDecrease(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakSuccessTensionDecrease;

            case EJumpScareIntensity.Medium:
                return mediumSuccessTensionDecrease;

            case EJumpScareIntensity.Strong:
                return strongSuccessTensionDecrease;
        }

        return 0.0f;
    }

    public float GetFailureTensionIncrease(EJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case EJumpScareIntensity.Weak:
                return weakFailureTensionIncrease;

            case EJumpScareIntensity.Medium:
                return mediumFailureTensionIncrease;

            case EJumpScareIntensity.Strong:
                return strongFailureTensionIncrease;
        }

        return 0.0f;
    }
}