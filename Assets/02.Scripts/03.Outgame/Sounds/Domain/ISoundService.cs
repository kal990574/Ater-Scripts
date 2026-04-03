using UnityEngine;
using UnityEngine.Audio;

public interface ISoundService
{

    void PlayBGM(string key, float fadeTime = 1f);
    void StopBGM(float fadeTime = 1f);

    void PlaySFX(string key, Vector3 position, float volume = 1f);

    void PlaySFX2D(string key, float volume = 1f);

    void PlayStinger(string key, float volume = 1f);

    void SetMasterVolume(float volume);
    void SetBGMVolume(float volume);
    void SetSFXVolume(float volume);
    void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.5f);

    void PauseAll();
    void ResumeAll();
}
