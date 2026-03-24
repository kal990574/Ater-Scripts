using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour, ISoundService
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;

    [Header("BGM")]
    [SerializeField] private AudioSource _bgmSourceA;
    [SerializeField] private AudioSource _bgmSourceB;

    [Header("Stinger")]
    [SerializeField] private AudioSource _stingerSource;


    private const string PARAM_MASTER = "MasterVolume";
    private const string PARAM_BGM = "BGMVolume";
    private const string PARAM_SFX = "SFXVolume";


    private const float SFX_PITCH_VARIANCE = 0.1f;

    
    private AudioSource _activeBGM;
    private AudioSource _inActiveBGM;
    private Coroutine _bgmFadeCoroutine;

    private void Awake()
    {
        _activeBGM = _bgmSourceA;
        _inActiveBGM = _bgmSourceB;
        _inActiveBGM.volume = 0f;
    }

    public void PlayBGM(AudioClip clip, float fadeTime = 1f)
    {
        if (clip == null) return;

        if (_activeBGM.clip == clip && _activeBGM.isPlaying) return;

        if(_bgmFadeCoroutine != null)
        {
            StopCoroutine( _bgmFadeCoroutine );
        }

        _bgmFadeCoroutine = StartCoroutine(CrossfadeBGM(clip, fadeTime));
    }


    public void StopBGM(float fadeTime = 1f)
    {
        if( _bgmFadeCoroutine != null)
        {
            StopCoroutine( _bgmFadeCoroutine );
        }
        _bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeTime));
    }



    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if( clip == null) return;

        GameObject tempgo = new GameObject("SFX_TEMP");
        tempgo.transform.position = position;

        AudioSource source = tempgo.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f + Random.Range(- SFX_PITCH_VARIANCE, SFX_PITCH_VARIANCE);
        source.spatialBlend = 1f;
        source.Play();

        Destroy(tempgo, clip.length / source.pitch + 0.1f);
    }

    public void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if(clip == null) return;

        GameObject tempgo = new GameObject("SFX2D_TEMP");
        AudioSource source = tempgo.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(tempgo, clip.length + 0.1f);
    }

    public void PlayStinger(AudioClip clip, float volume = 1f)
    {
        if(clip == null) return;
        _stingerSource.Stop();
        _stingerSource.clip = clip;
        _stingerSource.volume = volume;
        _stingerSource.Play();
    }

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

    private void SetMixerVolume(string paramName, float volume)
    {
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        _mixer.SetFloat(paramName, dB);
    }

    public void PauseAll()
    {
        _activeBGM.Pause();
        _stingerSource.Pause();
        AudioListener.pause = true;
    }
    public void ResumeAll()
    {
        _activeBGM.UnPause();
        _stingerSource.UnPause();
        AudioListener.pause = false;
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
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeTime;

            _activeBGM.volume = Mathf.Lerp(startVolume, 0f, t);
            _inActiveBGM.volume = Mathf.Lerp(0f,1f,t);

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

        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            _activeBGM.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
            yield return null;
        }
        _activeBGM.Stop();
        _activeBGM.volume = 0f;
    }
}
