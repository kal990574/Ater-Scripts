using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class SafeBoxInstance : MonoBehaviour, IPuzzleIntance
{
    [Header("Dial References")]
    [SerializeField] private SafeBoxDial _dial;
    [SerializeField] private SafeBoxButton _resetButton;
    [Header("UI")]
    [SerializeField] private TextMeshPro _codeText;

    [Header("Animation")]
    [SerializeField] private float _resetDuration = 0.2f;
    [SerializeField] private Ease _resetEase = Ease.OutQuad;

    [Header("Debug")]
    [SerializeField] private List<int> _currentInput = new();
    [SerializeField] private int _previewDialValue;

    private SafeBoxController _controller;
    private readonly SafeBoxStateMachine _stateMachine = new();
    private Tween _resetTween;

    public IReadOnlyList<int> CurrentInput => _stateMachine.CurrentInput;
    public int TargetInputCount => _stateMachine.TargetInputCount;
    public bool IsInputFull => _stateMachine.IsInputFull;
    public bool IsBusy => _stateMachine.IsBusy;

    public void Initialize(SafeBoxController controller, IReadOnlyList<int> correctCode)
    {
        _controller = controller;
        _stateMachine.Initialize(correctCode);
        SyncDebugState();

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

        _dial.ForceResetVisualImmediate();
        RefreshCodeText();
    }

    public bool CanAcceptDialCommit()
    {
        return _stateMachine.CanAcceptCommit();
    }

    public void SetPreviewDialValue(int value)
    {
        if (!_stateMachine.TrySetPreview(value))
        {
            return;
        }

        SyncDebugState();
        _controller?.OnDialSpin();
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
        if (_controller == null)
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
        if (_stateMachine.IsSolved)
        {
            return;
        }

        _stateMachine.MarkSolved();
        SyncDebugState();
        _controller?.HandlePuzzleSuccess(this);
        Destroy(gameObject);
    }

    [ContextMenu("Fail Puzzle")]
    public void FailPuzzle()
    {
        if (_stateMachine.IsSolved)
        {
            return;
        }

        _controller?.HandlePuzzleFail(this);
        ResetStateInternal(true);
    }

    public void ResetByButton()
    {
        if (_stateMachine.IsSolved)
        {
            return;
        }

        _controller?.HandlePuzzleReset(this);
        ResetStateInternal(true);
    }

    public void Cancel()
    {
        if (_stateMachine.IsSolved)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void ResetStateInternal(bool animated)
    {
        _stateMachine.Reset(animated && _dial != null);
        SyncDebugState();
        RefreshCodeText();

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

    private void OnResetAnimationCompleted()
    {
        _stateMachine.FinishResetAnimation();
        SyncDebugState();
        RefreshCodeText();
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
}
