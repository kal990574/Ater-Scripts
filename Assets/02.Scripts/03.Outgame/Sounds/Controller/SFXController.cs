using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SFXController : MonoBehaviour
{
    private enum PlaybackMode
    {
        UseSoundData,
        Force2D
    }

    [Header("Stinger")]
    [SerializeField] private AudioSource _stingerSource;

    [Header("SFX")]
    [SerializeField] private GameObject _sfxPrefab;
    [SerializeField] private GameObject _sfx2DPrefab;

    private const float SfxPitchVariance = 0.1f;
    private const float DespawnBuffer = 0.1f;

    private struct SFXEntry
    {
        public AudioSource Source;
        public Coroutine Coroutine;
    }

    private readonly List<SFXEntry> _activeSFX = new();

    private bool _isPaused;

    public void PlaySFX(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null) return;
        float pitch = 1f + Random.Range(-SfxPitchVariance, SfxPitchVariance);
        PlayAndDespawn(_sfxPrefab, clip, position, volume, pitch);
    }

    public void PlaySFX(SoundData soundData, Vector3 position, float volume = 1f)
    {
        if (soundData == null || soundData.AudioClip == null) return;

        float pitch = 1f + Random.Range(-SfxPitchVariance, SfxPitchVariance);
        float finalVolume = soundData.BaseVolume * volume;
        PlayAndDespawn(_sfxPrefab, soundData, position, finalVolume, pitch, PlaybackMode.UseSoundData);
    }

    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        GameObject sfxInstance = LeanPool.Spawn(_sfx2DPrefab, Vector3.zero, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f;
        source.Play();
        LeanPool.Despawn(sfxInstance, clip.length + DespawnBuffer);
    }

    public void PlaySFX2D(SoundData soundData, float volume = 1f)
    {
        if (soundData == null || soundData.AudioClip == null) return;

        float finalVolume = soundData.BaseVolume * volume;
        GameObject sfxInstance = LeanPool.Spawn(_sfx2DPrefab, Vector3.zero, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        ApplySoundData(source, soundData, PlaybackMode.Force2D);
        source.clip = soundData.AudioClip;
        source.volume = finalVolume;
        source.pitch = 1f;
        source.Play();
        LeanPool.Despawn(sfxInstance, soundData.AudioClip.length + DespawnBuffer);
    }

    private void PlayAndDespawn(GameObject prefab, AudioClip clip, Vector3 position, float volume, float pitch)
    {
        GameObject sfxInstance = LeanPool.Spawn(prefab, position, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();

        if (_isPaused)
        {
            source.Pause();
        }

        float duration = clip.length / pitch + DespawnBuffer;
        Coroutine coroutine = StartCoroutine(DespawnAfter(sfxInstance, source, duration));
        _activeSFX.Add(new SFXEntry { Source = source, Coroutine = coroutine });
    }

    private void PlayAndDespawn(GameObject prefab, SoundData soundData, Vector3 position, float volume, float pitch, PlaybackMode playbackMode)
    {
        GameObject sfxInstance = LeanPool.Spawn(prefab, position, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        ApplySoundData(source, soundData, playbackMode);
        source.clip = soundData.AudioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();

        if (_isPaused)
        {
            source.Pause();
        }

        float duration = soundData.AudioClip.length / pitch + DespawnBuffer;
        Coroutine coroutine = StartCoroutine(DespawnAfter(sfxInstance, source, duration));
        _activeSFX.Add(new SFXEntry { Source = source, Coroutine = coroutine });
    }

    private static void ApplySoundData(AudioSource source, SoundData soundData, PlaybackMode playbackMode)
    {
        if (source == null || soundData == null)
        {
            return;
        }

        if (playbackMode == PlaybackMode.Force2D)
        {
            source.spatialBlend = 0f;
            return;
        }

        source.spatialBlend = soundData.IsSpacialClip ? 1 : 0;
        if (soundData.IsSpacialClip)
        {
            source.minDistance = soundData.MinDistance;
            source.maxDistance = soundData.MaxDistance;
        }
        
    }

    private IEnumerator DespawnAfter(GameObject sfxInstance, AudioSource source, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!_isPaused)
            {
                elapsed += Time.unscaledDeltaTime;
            }

            yield return null;
        }

        _activeSFX.RemoveAll(entry => entry.Source == source);
        LeanPool.Despawn(sfxInstance);
    }

    public void PauseSFX()
    {
        _isPaused = true;
        foreach (var entry in _activeSFX)
        {
            entry.Source.Pause();
        }
    }

    public void ResumeSFX()
    {
        _isPaused = false;
        foreach (var entry in _activeSFX)
        {
            entry.Source.UnPause();
        }
    }

    private void OnDestroy()
    {
        foreach (var entry in _activeSFX)
        {
            if (entry.Coroutine != null)
            {
                StopCoroutine(entry.Coroutine);
            }
        }

        _activeSFX.Clear();
    }

    public void PlayStinger(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _stingerSource.Stop();
        _stingerSource.clip = clip;
        _stingerSource.volume = volume;
        _stingerSource.Play();
    }

    public void PlayStinger(SoundData soundData, float volume = 1f)
    {
        if (soundData == null || soundData.AudioClip == null) return;
        _stingerSource.Stop();
        _stingerSource.clip = soundData.AudioClip;
        _stingerSource.volume = soundData.BaseVolume * volume;
        _stingerSource.Play();
    }

    public void PauseStinger()
    {
        _stingerSource.Pause();
    }

    public void ResumeStinger()
    {
        _stingerSource.UnPause();
    }

    public AudioSource PlayLoopSFX(SoundData soundData, Vector3 position, float volume = 1f)
    {
        if (soundData == null || soundData.AudioClip == null) return null;

        float finalVolume = soundData.BaseVolume * volume;
        GameObject sfxInstance = LeanPool.Spawn(_sfxPrefab, position, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        ApplySoundData(source, soundData, PlaybackMode.UseSoundData);
        source.clip = soundData.AudioClip;
        source.volume = finalVolume;
        source.pitch = 1f;
        source.loop = true;
        source.Play();

        if (_isPaused) source.Pause();

        _activeSFX.Add(new SFXEntry { Source = source, Coroutine = null });
        return source;
    }

    public void StopLoopSFX(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.loop = false;
        _activeSFX.RemoveAll(entry => entry.Source == source);
        LeanPool.Despawn(source.gameObject);
    }
}
