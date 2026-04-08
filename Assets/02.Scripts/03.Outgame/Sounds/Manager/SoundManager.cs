using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour, ISoundService
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    [Header("Controller")]
    [SerializeField] private BGMController _bgmController;
    [SerializeField] private SFXController _sfxController;
    [SerializeField] private MixerController _mixerController;

    [Header("Data")]
    [SerializeField] private SoundDataTableSO _soundDataTableSO;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void PlayBGM(string key, float fadeTime = 1f)
    {
        SoundData soundData = GetSoundData(key);
        if (soundData == null || soundData.AudioClip == null) return;
        _bgmController.Play(soundData.AudioClip, fadeTime);
    }

    public void StopBGM(float fadeTime = 1f)
    {
        _bgmController.Stop(fadeTime);
    }

    public void PlaySFX(string key, Vector3 position, float volume = 1f)
    {
        SoundData soundData = GetSoundData(key);
        if (soundData == null || soundData.AudioClip == null) return;
        _sfxController.PlaySFX(soundData, position, volume);
    }

    public void PlaySFX2D(string key, float volume = 1f)
    {
        SoundData soundData = GetSoundData(key);
        if (soundData == null || soundData.AudioClip == null) return;
        _sfxController.PlaySFX2D(soundData, volume);
    }

    public void PlayStinger(string key, float volume = 1f)
    {
        SoundData soundData = GetSoundData(key);
        if (soundData == null || soundData.AudioClip == null) return;
        _sfxController.PlayStinger(soundData, volume);
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

    private SoundData GetSoundData(string key)
    {
        return _soundDataTableSO.GetSoundData(key);
    }

    public AudioSource PlayLoopSFX(string key, Vector3 position, float volume = 1f)
    {
        SoundData soundData = GetSoundData(key);
        if (soundData == null || soundData.AudioClip == null) return null;
        return _sfxController.PlayLoopSFX(soundData, position, volume);
    }

    public void StopLoopSFX(AudioSource source)
    {
        _sfxController.StopLoopSFX(source);
    }
}
