using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorReplaceMainJumpScare : MainJumpScareBase
{
    private const string FadeAmountPropertyName = "_FadeAmount";

    [Title("Hierarchy References")] 
    [SerializeField] private GameObject _doorWall;
    [SerializeField] private GameObject _doorRoot;
    [SerializeField] private GameObject _blockWallRoot;
    [SerializeField] private Transform _doorTransform;

    [Title("Door Tween")]
    [SerializeField] private Vector3 _doorClosedLocalEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
    [SerializeField] private float _doorCloseDuration = 0.3f;
    [SerializeField] private Ease _doorCloseEase = Ease.InQuad;

    [Title("Fade")]
    [SerializeField] private float _fadeStartDelay = 0.1f;
    [SerializeField] private float _fadeDuration = 0.6f;
    [SerializeField] private Ease _fadeEase = Ease.InOutQuad;

    [Title("Complete Options")]
    [SerializeField] private bool _disableDoorWallRootOnFadeComplete = true;

    [Title("Runtime - Auto Collected")]
    [SerializeField, ReadOnly] private List<AllInOneShaderController> _doorShaderControllers = new List<AllInOneShaderController>();
    [SerializeField, ReadOnly] private List<AllInOneShaderController> _blockWallShaderControllers = new List<AllInOneShaderController>();
    
    [Title("Runtime State")]
    [SerializeField, ReadOnly] private bool _isInitialized;
    [SerializeField, ReadOnly] private bool _isPlaying;
    [SerializeField, ReadOnly] private Sequence _sequence;

    private Vector3 _cachedDoorInitialLocalEulerAngles;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        ApplyInitialState();
    }

    protected virtual void OnDestroy()
    {
        KillSequence();
    }
    
    private void Initialize()
    {
        if (_doorTransform != null)
        {
            _cachedDoorInitialLocalEulerAngles = _doorTransform.localEulerAngles;
        }

        CollectDoorWallShaderControllers();
        CollectBlockWallShaderControllers();

        InitializeShaderControllers(_doorShaderControllers);
        InitializeShaderControllers(_blockWallShaderControllers);

        _isInitialized = true;
    }

    protected override void OnExecute()
    {
        if (_isPlaying == true)
        {
            return;
        }

        EnsureInitialized();

        _isPlaying = true;

        KillSequence();

        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(UpdateType.Normal);

        _sequence.Append(
            _doorTransform.DOLocalRotate(_doorClosedLocalEulerAngles, _doorCloseDuration)
                .SetEase(_doorCloseEase)
        );

        _sequence.AppendInterval(_fadeStartDelay);
        
        _sequence.Append(
            DOTween.To(
                    () => 0.0f,
                    value => ApplyFadeProgress(value),
                    1.0f,
                    _fadeDuration
                )
                .SetEase(_fadeEase)
        );

        _sequence.OnComplete(() =>
        {
            HandleFadeCompleted();
        });

        _sequence.OnKill(() =>
        {
            _sequence = null;
        });
    }

    public void ApplyInitialState()
    {
        EnsureInitialized();

        if (_doorTransform != null)
        {
            _doorTransform.localEulerAngles = _cachedDoorInitialLocalEulerAngles;
        }

        SetFadeAmount(_doorShaderControllers, 0.0f);
        SetFadeAmount(_blockWallShaderControllers, 1.0f);

        if (_doorWall != null)
        {
            _doorWall.SetActive(true);
        }
        
        if (_doorRoot != null)
        {
            _doorRoot.SetActive(true);
        }

        if (_blockWallRoot != null)
        {
            _blockWallRoot.SetActive(true);
        }

        _isPlaying = false;
    }
    
    public void ForceComplete()
    {
        if (_isPlaying == false)
        {
            return;
        }

        KillSequence();

        ApplyFadeProgress(1.0f);
        HandleFadeCompleted();
    }

    private void HandleFadeCompleted()
    {
        if (_disableDoorWallRootOnFadeComplete == true && _doorWall != null)
        {
            _doorWall.SetActive(false);
            _doorRoot.SetActive(false);
        }

        _isPlaying = false;
        NotifyFinished();
    }

    protected override void OnResetJumpScare()
    {
        KillSequence();
        ApplyInitialState();
    }

    private void ApplyFadeProgress(float progress)
    {
        float clampedProgress = Mathf.Clamp01(progress);

        SetFadeAmount(_doorShaderControllers, clampedProgress);
        SetFadeAmount(_blockWallShaderControllers, 1.0f - clampedProgress);
    }

    private void SetFadeAmount(List<AllInOneShaderController> controllers, float value)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            AllInOneShaderController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            controller.SetFloat(FadeAmountPropertyName, value);
        }
    }

    private void CollectDoorWallShaderControllers()
    {
        _doorShaderControllers.Clear();

        if (_doorRoot == null)
        {
            return;
        }

        AllInOneShaderController[] controllers = _doorRoot.GetComponentsInChildren<AllInOneShaderController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            AllInOneShaderController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            _doorShaderControllers.Add(controller);
        }
    }

    private void CollectBlockWallShaderControllers()
    {
        _blockWallShaderControllers.Clear();

        if (_blockWallRoot == null)
        {
            return;
        }

        AllInOneShaderController[] controllers = _blockWallRoot.GetComponentsInChildren<AllInOneShaderController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            AllInOneShaderController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            _blockWallShaderControllers.Add(controller);
        }
    }

    private void InitializeShaderControllers(List<AllInOneShaderController> controllers)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            AllInOneShaderController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            controller.Init();
        }
    }

    private void EnsureInitialized()
    {
        if (_isInitialized == true)
        {
            return;
        }

        Initialize();
    }

    private void KillSequence()
    {
        if (_sequence == null)
        {
            return;
        }

        if (_sequence.IsActive() == true)
        {
            _sequence.Kill();
        }

        _sequence = null;
    }
}
