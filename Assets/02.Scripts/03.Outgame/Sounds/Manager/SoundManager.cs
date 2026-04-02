using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour, ISoundService
{
    [Header("Controller")]
    [SerializeField] private BGMController _bgmController;
    [SerializeField] private SFXController _sfxController;
    [SerializeField] private MixerController _mixerController;

    [Header("Data")]
    [SerializeField] private SoundDataTableSO _soundDataTableSO;

    public void PlayBGM(string key, float fadeTime = 1f)
    {
        AudioClip clip = GetClip(key);
        if (clip == null) return;
        _bgmController.Play(clip, fadeTime);
    }
    public void StopBGM(float fadeTime = 1f)
    {
        _bgmController.Stop(fadeTime);
    }
    public void PlaySFX(string key, Vector3 position, float volume = 1f)
    {
        AudioClip clip = GetClip(key);
        if (clip == null) return;
        _sfxController.PlaySFX(clip, position, volume);
    }
    public void PlaySFX2D(string key, float volume = 1f)
    {
        AudioClip clip = GetClip(key);
        if (clip == null) return;
        _sfxController.PlaySFX2D(clip, volume);
    }
    public void PlayStinger(string key, float volume = 1f)
    {
        AudioClip clip = GetClip(key);
        if (clip == null) return;
        _sfxController.PlayStinger(clip, volume);
    }

    public void SetMasterVolume(float volume)
    {
        _mixerController.SetMasterVolume(volume);
    }
    public void SetBGMVolume(float volume)
    {
        _mixerController.SetBGMVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        _mixerController.SetSFXVolume(volume);
    }

    public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.5f)
    {
        _mixerController.TransitionToSnapshot(snapshot, transitionTime);
    }

    public void PauseAll()
    {
        _bgmController.Pause();
        _sfxController.PauseSFX();
        _sfxController.PauseStinger();
    }
    public void ResumeAll()
    {
        _bgmController.Resume();
        _sfxController.ResumeSFX();
        _sfxController.ResumeStinger();
    }
    private AudioClip GetClip(string key)
    {
        return _soundDataTableSO.GetClip(key);
    }

}
