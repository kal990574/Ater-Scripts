using Lean.Pool;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    [Header("Stinger")]
    [SerializeField] private AudioSource _stingerSource;

    [Header("SFX")]
    [SerializeField] private GameObject _sfxPrefab;
    [SerializeField] private GameObject _sfx2DPrefab;

    private const float SFX_PITCH_VARIANCE = 0.1f;

    public void PlaySFX(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null) return;

        GameObject tempgo = LeanPool.Spawn(_sfxPrefab, position, Quaternion.identity);

        AudioSource source = tempgo.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f + Random.Range(-SFX_PITCH_VARIANCE, SFX_PITCH_VARIANCE);
        source.Play();

        LeanPool.Despawn(tempgo, clip.length / source.pitch + 0.1f);
    }

    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        GameObject tempgo = LeanPool.Spawn(_sfx2DPrefab, Vector3.zero, Quaternion.identity);

        AudioSource source = tempgo.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f;
        source.Play();

        LeanPool.Despawn(tempgo, clip.length + 0.1f);
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