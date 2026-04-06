using UnityEngine;

public readonly struct SoundJumpScareExecutionRequest
{
    public readonly SoundSubJumpScareDefinitionSO Definition;
    public readonly Transform PlayerRootTransform;
    public readonly Transform PlayerCameraTransform;
    public readonly ISoundService SoundService;

    public SoundJumpScareExecutionRequest(
        SoundSubJumpScareDefinitionSO definition,
        Transform playerRootTransform,
        Transform playerCameraTransform,
        ISoundService soundService)
    {
        Definition = definition;
        PlayerRootTransform = playerRootTransform;
        PlayerCameraTransform = playerCameraTransform;
        SoundService = soundService;
    }

    public bool IsValid()
    {
        return Definition != null
               && PlayerRootTransform != null
               && PlayerCameraTransform != null
               && SoundService != null
               && Definition.HasAnyValidSoundEntry() == true;
    }
}