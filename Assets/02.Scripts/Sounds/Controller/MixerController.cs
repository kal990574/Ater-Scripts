using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;

    private const string PARAM_MASTER = "MasterVolume";
    private const string PARAM_BGM = "BGMVolume";
    private const string PARAM_SFX = "SFXVolume";

    public void SetMasterVolume(float volume)
    {
        SetMixerVolume(PARAM_MASTER, volume);
    }
    public void SetBGMVolume(float volume)
    {
        SetMixerVolume(PARAM_BGM, volume);
    }
    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(PARAM_SFX, volume);
    }

    public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
    {
        if (snapshot == null) return;

        snapshot.TransitionTo(transitionTime);
    }
    private void SetMixerVolume(string paramName, float volume)
    {
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        _mixer.SetFloat(paramName, dB);
    }

}