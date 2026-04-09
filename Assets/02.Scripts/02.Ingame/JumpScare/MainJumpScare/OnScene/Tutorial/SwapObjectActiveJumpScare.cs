using Sirenix.OdinInspector;
using UnityEngine;

public class SwapObjectActiveJumpScare :  MainJumpScareBase
{
    [Header("References")]
    [SerializeField] private GameObject _beforeObject;
    [SerializeField] private GameObject _afterObject;
    [SerializeField] private SoundSFXEmitter _soundEmitter;

    [Header("Default State")]
    [SerializeField] private bool _beforeObjectDefaultActive = true;
    [SerializeField] private bool _afterObjectDefaultActive = false;

    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;

    [Header("Cached Initial State")]
    [SerializeField, ReadOnly] private bool _initialBeforeObjectActive;
    [SerializeField, ReadOnly] private bool _initialAfterObjectActive;

    private bool _isInitialStateCached;

    protected override void Awake()
    {
        base.Awake();

        CacheInitialState();
        ApplyDefaultState();
    }

    private void Reset()
    {
    }
    
    protected override void OnExecute()
    {
        if (_beforeObject != null)
        {
            _beforeObject.SetActive(false);
        }

        if (_afterObject != null)
        {
            _afterObject.SetActive(true);
        }

        PlaySound();

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 오브젝트 전환형 메인 점프스케어를 실행했습니다.", this);
        }

        NotifyFinished();
    }

    protected override void OnResetJumpScare()
    {
        if (_isInitialStateCached == false)
        {
            CacheInitialState();
        }

        ApplyDefaultState();

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 오브젝트 전환형 메인 점프스케어를 리셋했습니다.", this);
        }
    }

    [Button]
    public void ApplyCachedInitialState()
    {
        if (_isInitialStateCached == false)
        {
            CacheInitialState();
        }

        if (_beforeObject != null)
        {
            _beforeObject.SetActive(_initialBeforeObjectActive);
        }

        if (_afterObject != null)
        {
            _afterObject.SetActive(_initialAfterObjectActive);
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 캐시된 초기 상태를 적용했습니다.", this);
        }
    }

    private void CacheInitialState()
    {
        _initialBeforeObjectActive = _beforeObject != null && _beforeObject.activeSelf;
        _initialAfterObjectActive = _afterObject != null && _afterObject.activeSelf;
        _isInitialStateCached = true;
    }

    private void ApplyDefaultState()
    {
        if (_beforeObject != null)
        {
            _beforeObject.SetActive(_beforeObjectDefaultActive);
        }

        if (_afterObject != null)
        {
            _afterObject.SetActive(_afterObjectDefaultActive);
        }
    }

    private void PlaySound()
    {
        if (_soundEmitter == null)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] SoundSFXEmitter 가 없어 사운드를 재생하지 않습니다.", this);
            }

            return;
        }

        _soundEmitter.Play();
    }
}
