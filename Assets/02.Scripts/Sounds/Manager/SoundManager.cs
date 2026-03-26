using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour, ISoundService
{
    [Header("Controller")]
    [SerializeField] private BGMController _bgmController;
    [SerializeField] private SFXController _sfxController;
    [SerializeField] private MixerController _mixerController;

    public void PlayBGM(AudioClip clip, float fadeTime = 1f)
    {
        _bgmController.Play(clip, fadeTime);
    }
    public void StopBGM(float fadeTime = 1f)
    {
        _bgmController.Stop(fadeTime);
    }
    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        _sfxController.PlaySFX(clip, position, volume);
    }
    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        _sfxController.PlaySFX2D(clip, volume);
    }
    public void PlayStinger(AudioClip clip, float volume = 1f)
    {
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
        AudioListener.pause = true;
    }
    public void ResumeAll()
    {
        _bgmController.Resume();
        _sfxController.ResumeSFX();
        _sfxController.ResumeStinger();
        AudioListener.pause = false;
    }

}
