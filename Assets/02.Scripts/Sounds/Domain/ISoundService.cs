using UnityEngine;
using UnityEngine.Audio;

public interface ISoundService
{

    void PlayBGM(AudioClip clip, float fadeTime = 1f);
    void StopBGM(float fadeTime = 1f);

    void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f);

    void PlaySFX2D(AudioClip clip, float volume = 1f);

    void PlayStinger(AudioClip clip, float volume = 1f);

    void SetMasterVolume(float volume);
    void SetBGMVolume(float volume);
    void SetSFXVolume(float volume);
    void TransitionToSnapshot(AudioMixerSnapshot Snapshot, float transitionTime = 0.5f);

    void PauseAll();
    void ResumeAll();
}
