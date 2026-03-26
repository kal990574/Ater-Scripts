using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SFXController : MonoBehaviour
{
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

    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        PlayAndDespawn(_sfx2DPrefab, clip, Vector3.zero, volume, 1f);
    }

    private void PlayAndDespawn(GameObject prefab, AudioClip clip, Vector3 position, float volume, float pitch)
    {
        GameObject sfxInstance = LeanPool.Spawn(prefab, position, Quaternion.identity);
        AudioSource source = sfxInstance.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();

        if(_isPaused)
        {
            source.Pause();
        }

        float duration = clip.length / pitch + DespawnBuffer;
        Coroutine coroutine = StartCoroutine(DespawnAfter(sfxInstance, source, duration));
        _activeSFX.Add(new SFXEntry { Source = source, Coroutine = coroutine });
    }

    private IEnumerator DespawnAfter(GameObject sfxInstance, AudioSource source, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!_isPaused)
                elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _activeSFX.RemoveAll(entry => entry.Source == source);
        LeanPool.Despawn(sfxInstance);
    }

    public void PauseSFX()
    {
        _isPaused = true;
        foreach (var entry in _activeSFX)
            entry.Source.Pause();
    }

    public void ResumeSFX()
    {
        _isPaused = false;
        foreach (var entry in _activeSFX)
            entry.Source.UnPause();
    }

    private void OnDestroy()
    {
        foreach (var entry in _activeSFX)
        {
            if (entry.Coroutine != null)
                StopCoroutine(entry.Coroutine);
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

    public void PauseStinger()
    {
        _stingerSource.Pause();
    }

    public void ResumeStinger()
    {
        _stingerSource.UnPause();
    }
}