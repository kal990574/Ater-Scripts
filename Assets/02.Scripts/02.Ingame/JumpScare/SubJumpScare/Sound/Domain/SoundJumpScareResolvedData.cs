using UnityEngine;

public readonly struct SoundJumpScareResolvedData
{
    public readonly string SoundKey;
    public readonly Vector3 WorldPosition;
    public readonly float Volume;

    public SoundJumpScareResolvedData(
        string soundKey,
        Vector3 worldPosition,
        float volume)
    {
        SoundKey = soundKey;
        WorldPosition = worldPosition;
        Volume = volume;
    }

    public bool IsValid()
    {
        return string.IsNullOrWhiteSpace(SoundKey) == false;
    }
}