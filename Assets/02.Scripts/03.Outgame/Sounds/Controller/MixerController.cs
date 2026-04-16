using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;

    private const string ParamMaster = "Master";
    private const string ParamBGM = "Music";
    private const string ParamSFX = "SFX";
    private const string ParamHint = "Hint";

    private const float MinVolumeForLog = 0.0001f;
    private const float DecibelMultiplier = 20f;
    private const float MinDecibels = -80f;

    public void SetMasterVolume(float volume)
    {
        SetMixerVolume(ParamMaster, volume);
    }
    public void SetBGMVolume(float volume)
    {
        SetMixerVolume(ParamBGM, volume);
    }
    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(ParamSFX, volume);
    }
    public void SetHintVolume(float volume)
    {
        SetMixerVolume(ParamHint, volume);
    }

    public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
    {
        if (snapshot == null) return;

        snapshot.TransitionTo(transitionTime);
    }
    private void SetMixerVolume(string paramName, float volume)
    {
        float dB = volume > MinVolumeForLog ? Mathf.Log10(volume) * DecibelMultiplier : MinDecibels;
        _mixer.SetFloat(paramName, dB);
    }

}