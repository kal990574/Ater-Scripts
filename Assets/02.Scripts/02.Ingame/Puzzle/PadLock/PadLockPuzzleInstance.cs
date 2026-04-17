using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class PadLockPuzzleInstance : MonoBehaviour, IPuzzleIntance
{
    private const float SuccessDelay = 0.5f;
    private const float FailDelay = 0.5f;

    [Header("Rows")]
    [SerializeField] private int _row1 = 1;
    [SerializeField] private int _row2 = 1;
    [SerializeField] private int _row3 = 1;
    [SerializeField] private int _row4 = 1;

    [Header("Evaluation")]
    [SerializeField] private bool _checkOnRowClick = false;
    [SerializeField] private bool _invokeFailOnWrongCode = true;
    [SerializeField] private bool _destroyOnSuccess = true;

    [Header("Feedback")]
    [SerializeField] private Transform _shakeTarget;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private Vector3 _shakeStrength = new Vector3(0.03f, 0f, 0f);
    [SerializeField] private int _shakeVibrato = 20;
    [SerializeField] private Transform _shackleTransform;
    [SerializeField] private Vector3 _shackleOpenLocalOffset = new Vector3(0f, 0.02f, 0f);
    [SerializeField] private float _shackleOpenDuration = 0.25f;
    [SerializeField] private float _shackleRotateDuration = 0.2f;
    [SerializeField] private Vector3 _shackleOpenRotateEuler = new Vector3(0f, 0f, 180f);
    
    [SerializeField] private SoundSFXEmitter _successSfxEmitter;
    [SerializeField] private SoundSFXEmitter _failSfxEmitter;
    [SerializeField] private SoundSFXEmitter _spinSfxEmitter;
    
    private PadLockController _owner;
    private string _correctCode;
    private bool _isSolved;
    private bool _isInputLocked;
    private Tween _feedbackTween;
    private Tween _shakeTween;
    private Tween _shackleTween;
    private PadLockRowSelector[] _rowSelectors;
    private Vector3 _defaultShakeLocalPosition;
    private Vector3 _defaultShackleLocalPosition;
    private Quaternion _defaultShackleLocalRotation;

    public void Initialize(PadLockController owner, string correctCode)
    {
        _owner = owner;
        _correctCode = correctCode ?? string.Empty;
        _rowSelectors = GetComponentsInChildren<PadLockRowSelector>(true);

        foreach (PadLockRowSelector rowSelector in _rowSelectors)
        {
            rowSelector.Bind(this);
        }

        Transform shakeTarget = ResolveShakeTarget();
        if (shakeTarget != null)
        {
            _defaultShakeLocalPosition = shakeTarget.localPosition;
        }

        if (_shackleTransform != null)
        {
            _defaultShackleLocalPosition = _shackleTransform.localPosition;
            _defaultShackleLocalRotation = _shackleTransform.localRotation;
        }

        ResetRowsToDefault();
        _isInputLocked = false;
    }

    public bool CanAcceptInput()
    {
        return !_isInputLocked && !_isSolved;
    }

    public void SetRowValue(PadLockRowSelector.RowIndex rowIndex, int value)
    {
        if (!CanAcceptInput())
        {
            return;
        }

        _spinSfxEmitter?.Play();
        
        switch (rowIndex)
        {
            case PadLockRowSelector.RowIndex.Row1:
                _row1 = value;
                break;
            case PadLockRowSelector.RowIndex.Row2:
                _row2 = value;
                break;
            case PadLockRowSelector.RowIndex.Row3:
                _row3 = value;
                break;
            case PadLockRowSelector.RowIndex.Row4:
                _row4 = value;
                break;
        }
    }

    public void TryEvaluate()
    {
        if (!CanAcceptInput() || _owner == null)
        {
            return;
        }

        if (BuildCurrentCode() == _correctCode)
        {
            PlaySuccessSequence();
            return;
        }

        if (_invokeFailOnWrongCode)
        {
            PlayFailSequence();
            _owner.HandlePuzzleFail(this);
        }
    }

    public void Cancel()
    {
        if (_isSolved || _isInputLocked)
        {
            return;
        }

        Destroy(gameObject);
    }

    private string BuildCurrentCode()
    {
        return $"{_row1:0}{_row2:0}{_row3:0}{_row4:0}";
    }

    private void PlaySuccessSequence()
    {
        _isSolved = true;
        _isInputLocked = true;
        _successSfxEmitter?.Play();
        PlayShackleOpen();

        KillFeedbackTweenOnly();
        _feedbackTween = DOVirtual.DelayedCall(SuccessDelay, () =>
        {
            _owner.HandlePuzzleSuccess(this);

            if (_destroyOnSuccess)
            {
                Destroy(gameObject);
            }
        });
    }

    private void PlayFailSequence()
    {
        _isInputLocked = true;
        _failSfxEmitter?.Play();
        PlayShake();
        ResetRowsToDefaultAnimated(FailDelay);

        KillFeedbackTweenOnly();
        _feedbackTween = DOVirtual.DelayedCall(FailDelay, () =>
        {
            _isInputLocked = false;
        });
    }

    private void ResetRowsToDefault()
    {
        _row1 = 1;
        _row2 = 1;
        _row3 = 1;
        _row4 = 1;

        if (_rowSelectors == null)
        {
            return;
        }

        foreach (PadLockRowSelector rowSelector in _rowSelectors)
        {
            rowSelector.ResetToDefault();
        }
    }

    private void ResetRowsToDefaultAnimated(float duration)
    {
        _row1 = 1;
        _row2 = 1;
        _row3 = 1;
        _row4 = 1;

        if (_rowSelectors == null)
        {
            return;
        }

        foreach (PadLockRowSelector rowSelector in _rowSelectors)
        {
            rowSelector.AnimateResetToDefault(duration);
        }
    }

    private void PlayShake()
    {
        Transform shakeTarget = ResolveShakeTarget();
        if (shakeTarget == null)
        {
            return;
        }

        if (_shakeTween != null && _shakeTween.IsActive())
        {
            _shakeTween.Kill(false);
        }

        shakeTarget.localPosition = _defaultShakeLocalPosition;
        _shakeTween = shakeTarget.DOShakePosition(_shakeDuration, _shakeStrength, _shakeVibrato)
            .SetRelative(true)
            .OnKill(() => shakeTarget.localPosition = _defaultShakeLocalPosition)
            .OnComplete(() => shakeTarget.localPosition = _defaultShakeLocalPosition);
    }

    private void PlayShackleOpen()
    {
        if (_shackleTransform == null)
        {
            return;
        }

        if (_shackleTween != null && _shackleTween.IsActive())
        {
            _shackleTween.Kill(false);
        }

        _shackleTransform.localPosition = _defaultShackleLocalPosition;
        _shackleTransform.localRotation = _defaultShackleLocalRotation;

        Sequence shackleSequence = DOTween.Sequence();
        shackleSequence.Append(_shackleTransform.DOLocalMove(
            _defaultShackleLocalPosition + _shackleOpenLocalOffset,
            _shackleOpenDuration));
        shackleSequence.Append(_shackleTransform.DOLocalRotate(
            _shackleOpenRotateEuler,
            _shackleRotateDuration,
            RotateMode.LocalAxisAdd));
        _shackleTween = shackleSequence;
    }

    private Transform ResolveShakeTarget()
    {
        if (_shakeTarget != null)
        {
            return _shakeTarget;
        }

        return transform;
    }
    
    
    private void KillFeedbackTweenOnly()
    {
        if (_feedbackTween != null && _feedbackTween.IsActive())
        {
            _feedbackTween.Kill(false);
        }

        _feedbackTween = null;
    }

    private void OnDestroy()
    {
        KillFeedbackTweenOnly();

        if (_shakeTween != null && _shakeTween.IsActive())
        {
            _shakeTween.Kill(false);
        }

        if (_shackleTween != null && _shackleTween.IsActive())
        {
            _shackleTween.Kill(false);
        }

        Transform shakeTarget = ResolveShakeTarget();
        if (shakeTarget != null)
        {
            shakeTarget.localPosition = _defaultShakeLocalPosition;
        }

        if (_shackleTransform != null)
        {
            _shackleTransform.localPosition = _defaultShackleLocalPosition;
            _shackleTransform.localRotation = _defaultShackleLocalRotation;
        }

        if (_owner != null && !_isSolved)
        {
            _owner.ClearActiveInstance(this);
        }
    }
}
