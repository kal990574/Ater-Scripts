using System.Collections;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSourceA;
    [SerializeField] private AudioSource _bgmSourceB;

    private AudioSource _activeBGM;
    private AudioSource _inActiveBGM;
    private Coroutine _bgmFadeCoroutine;

    private bool _isPaused;
    private float _targetVolume = 1f;

    private const float MinFadeTime = 0.01f;

    private void Awake()
    {
        _activeBGM = _bgmSourceA;
        _inActiveBGM = _bgmSourceB;
        _inActiveBGM.volume = 0f;
    }

    public void Play(AudioClip clip, float fadeTime, float volume = 1f)
    {
        if (clip == null) return;

        if (_activeBGM.clip == clip && _activeBGM.isPlaying) return;

        _targetVolume = volume;
        fadeTime = Mathf.Max(fadeTime, MinFadeTime); //fadetime이 0일 때의 예외처리

        if (_bgmFadeCoroutine != null)
        {
            StopCoroutine(_bgmFadeCoroutine);
        }

        _bgmFadeCoroutine = StartCoroutine(CrossfadeBGM(clip, fadeTime));
    }

    public void Stop(float fadeTime)
    {
        fadeTime = Mathf.Max(fadeTime, MinFadeTime); //fadetime이 0일 때의 예외처리

        if (_bgmFadeCoroutine != null)
        {
            StopCoroutine(_bgmFadeCoroutine);
        }
        _bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeTime));
    }

    private IEnumerator CrossfadeBGM(AudioClip newClip, float fadeTime)
    {
        _inActiveBGM.clip = newClip;
        _inActiveBGM.volume = 0f;
        _inActiveBGM.Play();

        float elapsed = 0f;
        float startVolume = _activeBGM.volume;

        while (elapsed < fadeTime)
        {
            if (!_isPaused)
            {
                elapsed += Time.unscaledDeltaTime;
            }

            float t = elapsed / fadeTime;

            _activeBGM.volume = Mathf.Lerp(startVolume, 0f, t);
            _inActiveBGM.volume = Mathf.Lerp(0f, _targetVolume, t);

            yield return null;
        }

        _activeBGM.Stop();
        _activeBGM.volume = 0f;

        (_activeBGM, _inActiveBGM) = (_inActiveBGM, _activeBGM);
    }

    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float elapsed = 0f;
        float startVolume = _activeBGM.volume;
        float inActiveStartVolume = _inActiveBGM.volume;

        while (elapsed < fadeTime)
        {
            if (!_isPaused)
            {
                elapsed += Time.unscaledDeltaTime;
            }

            float t = elapsed / fadeTime;

            _activeBGM.volume = Mathf.Lerp(startVolume, 0f, t);
            _inActiveBGM.volume = Mathf.Lerp(inActiveStartVolume, 0f, t);

            yield return null;
        }

        _activeBGM.Stop();
        _activeBGM.volume = 0f;
        _inActiveBGM.Stop();
        _inActiveBGM.volume = 0f;
    }

    public void Pause()
    {
        _isPaused = true;
        _activeBGM.Pause();
    }

    public void Resume()
    {
        _isPaused = false;
        _activeBGM.UnPause();
    }
}