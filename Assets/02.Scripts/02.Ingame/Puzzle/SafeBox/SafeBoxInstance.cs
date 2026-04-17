using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class SafeBoxInstance : MonoBehaviour, IPuzzleIntance
{
    private const float SuccessDelay = 1f;

    [Header("Dial References")]
    [SerializeField] private SafeBoxDial _dial;
    [SerializeField] private SafeBoxButton _resetButton;

    [Header("UI")]
    [SerializeField] private TextMeshPro _codeText;

    [Header("Animation")]
    [SerializeField] private float _resetDuration = 0.2f;
    [SerializeField] private Ease _resetEase = Ease.OutQuad;

    [Header("Feedback")]
    [SerializeField] private Color _successTextColor = Color.green;
    [SerializeField] private Color _failTextColor = Color.red;
    [SerializeField] private Transform _textShakeTarget;
    [SerializeField] private float _textShakeDuration = 0.3f;
    [SerializeField] private Vector3 _textShakeStrength = new Vector3(0.03f, 0f, 0f);
    [SerializeField] private int _textShakeVibrato = 20;
    [SerializeField] private SoundSFXEmitter _successSfxEmitter;
    [SerializeField] private SoundSFXEmitter _failSfxEmitter;
    [SerializeField] private SoundSFXEmitter _dialSfxEmitter;
    [SerializeField] private SoundSFXEmitter _resetSfxEmitter;
    [SerializeField] private Transform _handleTransform;
    [SerializeField] private Vector3 _handleRotateEuler = new Vector3(0f, 0f, -45f);
    [SerializeField] private float _handleRotateDuration = 0.25f;

    [Header("Debug")]
    [SerializeField] private List<int> _currentInput = new();
    [SerializeField] private int _previewDialValue;

    private SafeBoxController _controller;
    private readonly SafeBoxStateMachine _stateMachine = new();
    private Tween _resetTween;
    private Tween _feedbackTween;
    private Tween _textShakeTween;
    private Tween _handleTween;
    private bool _isInputLocked;
    private Color _defaultTextColor;
    private Vector3 _defaultTextShakeLocalPosition;
    private Quaternion _defaultHandleLocalRotation;

    public IReadOnlyList<int> CurrentInput => _stateMachine.CurrentInput;
    public int TargetInputCount => _stateMachine.TargetInputCount;
    public bool IsInputFull => _stateMachine.IsInputFull;
    public bool IsBusy => _stateMachine.IsBusy || _isInputLocked;

    public void Initialize(SafeBoxController controller, IReadOnlyList<int> correctCode)
    {
        _controller = controller;
        _stateMachine.Initialize(correctCode);
        SyncDebugState();
        _defaultTextColor = _codeText != null ? _codeText.color : Color.white;

        if (_dial == null)
        {
            _dial = GetComponentInChildren<SafeBoxDial>(true);
        }

        if (_dial == null)
        {
            Debug.LogWarning($"[{nameof(SafeBoxInstance)}] {gameObject.name} is missing {nameof(SafeBoxDial)}.", this);
            return;
        }

        _dial.Bind(this);
        _resetButton?.Bind(this);

        Transform textShakeTarget = ResolveTextShakeTarget();
        if (textShakeTarget != null)
        {
            _defaultTextShakeLocalPosition = textShakeTarget.localPosition;
        }

        if (_handleTransform != null)
        {
            _defaultHandleLocalRotation = _handleTransform.localRotation;
        }

        _dial.ForceResetVisualImmediate();
        RefreshCodeText();
        ResetVisualFeedback();
        _isInputLocked = false;
    }

    public bool CanAcceptDialCommit()
    {
        return !_isInputLocked && _stateMachine.CanAcceptCommit();
    }

    public void SetPreviewDialValue(int value)
    {
        if (!_stateMachine.TrySetPreview(value))
        {
            return;
        }

        SyncDebugState();
        _dialSfxEmitter?.Play();
        RefreshCodeText();
    }

    public void CommitDialValue(int value)
    {
        if (!_stateMachine.TryCommit(value))
        {
            return;
        }

        SyncDebugState();
        RefreshCodeText();
    }

    public void TryEvaluate()
    {
        if (_controller == null || _isInputLocked)
        {
            return;
        }

        switch (_stateMachine.Evaluate())
        {
            case SafeBoxEvaluationResult.Fail:
                FailPuzzle();
                return;

            case SafeBoxEvaluationResult.Success:
                CompletePuzzle();
                return;
        }
    }

    [ContextMenu("Complete Puzzle")]
    public void CompletePuzzle()
    {
        if (_stateMachine.IsSolved || _isInputLocked)
        {
            return;
        }

        _isInputLocked = true;
        _stateMachine.MarkSolved();
        SyncDebugState();
        ApplyCodeTextColor(_successTextColor);
        _successSfxEmitter?.Play();
        ResetDialVisualOnly(true);
        PlayHandleRotate();

        KillFeedbackTweenOnly();
        _feedbackTween = DOVirtual.DelayedCall(SuccessDelay, () =>
        {
            _controller?.HandlePuzzleSuccess(this);
            Destroy(gameObject);
        });
    }

    [ContextMenu("Fail Puzzle")]
    public void FailPuzzle()
    {
        if (_stateMachine.IsSolved || _isInputLocked)
        {
            return;
        }

        _isInputLocked = true;
        ApplyCodeTextColor(_failTextColor);
        PlayTextShake();
        _failSfxEmitter?.Play();
        _controller?.HandlePuzzleFail(this);
        RefreshCodeText();
        ResetDialView(true);

        KillFeedbackTweenOnly();
        _feedbackTween = DOVirtual.DelayedCall(_resetDuration, () =>
        {
            ResetVisualFeedback();
            _isInputLocked = false;
        });
    }

    public void ResetByButton()
    {
        if (_stateMachine.IsSolved || _isInputLocked)
        {
            return;
        }

       _resetSfxEmitter?.Play();
        RefreshCodeText();
        ResetDialView(true);
    }

    public void Cancel()
    {
        if (_stateMachine.IsSolved || _isInputLocked)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void ResetDialView(bool animated)
    {
        _stateMachine.Reset(animated && _dial != null);
        SyncDebugState();
        
        if (_dial == null)
        {
            return;
        }

        _resetTween?.Kill();
        if (!animated)
        {
            _dial.ForceResetVisualImmediate();
            _stateMachine.FinishResetAnimation();
            SyncDebugState();
            return;
        }

        _resetTween = _dial.PlayResetAnimation(_resetDuration, _resetEase, OnResetAnimationCompleted);
    }

    private void ResetDialVisualOnly(bool animated)
    {
        if (_dial == null)
        {
            return;
        }

        _resetTween?.Kill();
        if (!animated)
        {
            _dial.ForceResetVisualImmediate();
            _resetTween = null;
            return;
        }

        _resetTween = _dial.PlayResetAnimation(_resetDuration, _resetEase, OnDialVisualResetCompleted);
    }

    private void OnResetAnimationCompleted()
    {
        _stateMachine.FinishResetAnimation();
        SyncDebugState();
        RefreshCodeText();
        _resetTween = null;
    }

    private void OnDialVisualResetCompleted()
    {
        _resetTween = null;
    }

    private void RefreshCodeText()
    {
        if (_codeText == null)
        {
            return;
        }

        if (TargetInputCount <= 0)
        {
            _codeText.text = string.Empty;
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder(TargetInputCount * 3);

        for (int i = 0; i < TargetInputCount; i++)
        {
            if (i < CurrentInput.Count)
            {
                builder.Append(CurrentInput[i]);
            }
            else if (i == CurrentInput.Count && !IsInputFull)
            {
                builder.Append(_stateMachine.PreviewDialValue);
            }
            else
            {
                builder.Append('-');
            }

            if (i < TargetInputCount - 1)
            {
                builder.Append(' ');
            }
        }

        _codeText.text = builder.ToString();
    }

    private void OnDestroy()
    {
        _resetTween?.Kill();
        KillFeedbackTweenOnly();

        if (_textShakeTween != null && _textShakeTween.IsActive())
        {
            _textShakeTween.Kill(false);
        }

        if (_handleTween != null && _handleTween.IsActive())
        {
            _handleTween.Kill(false);
        }

        ResetVisualFeedback();

        if (_controller != null && !_stateMachine.IsSolved)
        {
            _controller.ClearActiveInstance(this);
        }
    }

    public Camera GetCamera()
    {
        if (_controller != null)
        {
            return _controller.PuzzleCamera;
        }

        return Camera.main;
    }

    private void SyncDebugState()
    {
        _currentInput.Clear();
        for (int i = 0; i < _stateMachine.CurrentInput.Count; i++)
        {
            _currentInput.Add(_stateMachine.CurrentInput[i]);
        }

        _previewDialValue = _stateMachine.PreviewDialValue;
    }

    private void ApplyCodeTextColor(Color color)
    {
        if (_codeText == null)
        {
            return;
        }

        _codeText.color = color;
    }

    private void PlayTextShake()
    {
        Transform textShakeTarget = ResolveTextShakeTarget();
        if (textShakeTarget == null)
        {
            return;
        }

        if (_textShakeTween != null && _textShakeTween.IsActive())
        {
            _textShakeTween.Kill(false);
        }

        textShakeTarget.localPosition = _defaultTextShakeLocalPosition;
        _textShakeTween = textShakeTarget.DOShakePosition(_textShakeDuration, _textShakeStrength, _textShakeVibrato)
            .SetRelative(true)
            .OnKill(() => textShakeTarget.localPosition = _defaultTextShakeLocalPosition)
            .OnComplete(() => textShakeTarget.localPosition = _defaultTextShakeLocalPosition);
    }

    private void PlayHandleRotate()
    {
        if (_handleTransform == null)
        {
            return;
        }

        if (_handleTween != null && _handleTween.IsActive())
        {
            _handleTween.Kill(false);
        }

        _handleTransform.localRotation = _defaultHandleLocalRotation;
        _handleTween = _handleTransform.DOLocalRotate(_handleRotateEuler, _handleRotateDuration, RotateMode.LocalAxisAdd);
    }

    private void ResetVisualFeedback()
    {
        ApplyCodeTextColor(_defaultTextColor);

        Transform textShakeTarget = ResolveTextShakeTarget();
        if (textShakeTarget != null)
        {
            textShakeTarget.localPosition = _defaultTextShakeLocalPosition;
        }

        if (_handleTransform != null && !_stateMachine.IsSolved)
        {
            _handleTransform.localRotation = _defaultHandleLocalRotation;
        }
    }

    private Transform ResolveTextShakeTarget()
    {
        if (_textShakeTarget != null)
        {
            return _textShakeTarget;
        }

        if (_codeText != null)
        {
            return _codeText.transform;
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
}
