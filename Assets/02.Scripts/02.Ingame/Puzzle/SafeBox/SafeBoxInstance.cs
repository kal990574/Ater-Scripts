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
    private readonly List<int> _correctCode = new();
    private bool _isSolved;
    private bool _isResetAnimating;
    private Tween _resetTween;

    public IReadOnlyList<int> CurrentInput => _currentInput;
    public int TargetInputCount => _correctCode.Count;
    public bool IsInputFull => _currentInput.Count >= _correctCode.Count;
    public bool IsBusy => _isSolved || _isResetAnimating;

    public void Initialize(SafeBoxController controller, IReadOnlyList<int> correctCode)
    {
        _controller = controller;

        _correctCode.Clear();
        if (correctCode != null)
        {
            for (int i = 0; i < correctCode.Count; i++)
            {
                _correctCode.Add(Mathf.Clamp(correctCode[i], 0, 15));
            }
        }

        _currentInput.Clear();
        _previewDialValue = 0;

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
        _resetButton.Bind(this);

        _dial.ForceResetVisualImmediate();
        RefreshCodeText();
    }

    public bool CanAcceptDialCommit()
    {
        if (_isSolved)
        {
            return false;
        }

        if (_isResetAnimating)
        {
            return false;
        }

        if (_correctCode.Count == 0)
        {
            return false;
        }

        return _currentInput.Count < _correctCode.Count;
    }

    public void SetPreviewDialValue(int value)
    {
        if (_isSolved)
        {
            return;
        }

        if (_isResetAnimating)
        {
            return;
        }

        if (_correctCode.Count == 0)
        {
            return;
        }

        if (_currentInput.Count >= _correctCode.Count)
        {
            return;
        }

        _previewDialValue = Mathf.Clamp(value, 0, 15);
        _controller.OnDialSpin();
        RefreshCodeText();
    }

    public void CommitDialValue(int value)
    {
        if (!CanAcceptDialCommit())
        {
            return;
        }

        int clampedValue = Mathf.Clamp(value, 0, 15);
        _currentInput.Add(clampedValue);
        _previewDialValue = 0;
        RefreshCodeText();
    }

    public void TryEvaluate()
    {
        if (_isSolved || _isResetAnimating || _controller == null)
        {
            return;
        }

        if (_currentInput.Count != _correctCode.Count)
        {
            FailPuzzle();
            return;
        }

        for (int i = 0; i < _correctCode.Count; i++)
        {
            if (_currentInput[i] != _correctCode[i])
            {
                FailPuzzle();
                return;
            }
        }

        CompletePuzzle();
    }

    [ContextMenu("Complete Puzzle")]
    public void CompletePuzzle()
    {
        if (_isSolved)
        {
            return;
        }

        _isSolved = true;
        _controller?.HandlePuzzleSuccess(this);
        Destroy(gameObject);
    }

    [ContextMenu("Fail Puzzle")]
    public void FailPuzzle()
    {
        if (_isSolved)
        {
            return;
        }

        _controller?.HandlePuzzleFail(this);
        ResetStateInternal(true);
    }

    public void ResetByButton()
    {
        if (_isSolved)
        {
            return;
        }

        _controller?.HandlePuzzleReset(this);
        ResetStateInternal(true);
    }

    public void Cancel()
    {
        if (_isSolved)
        {
            return;
        }

        Destroy(gameObject);
    }

    private void ResetStateInternal(bool animated)
    {
        _currentInput.Clear();
        _previewDialValue = 0;
        RefreshCodeText();

        if (_dial == null)
        {
            return;
        }

        _resetTween?.Kill();
        _isResetAnimating = true;

        if (!animated)
        {
            _dial.ForceResetVisualImmediate();
            _isResetAnimating = false;
            return;
        }

        _resetTween = _dial.PlayResetAnimation(_resetDuration, _resetEase, OnResetAnimationCompleted);
    }

    private void OnResetAnimationCompleted()
    {
        _previewDialValue = 0;
        RefreshCodeText();
        _isResetAnimating = false;
        _resetTween = null;
    }

    private void RefreshCodeText()
    {
        if (_codeText == null)
        {
            return;
        }

        if (_correctCode.Count <= 0)
        {
            _codeText.text = string.Empty;
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder(_correctCode.Count * 3);

        for (int i = 0; i < _correctCode.Count; i++)
        {
            if (i < _currentInput.Count)
            {
                builder.Append(_currentInput[i]);
            }
            else if (i == _currentInput.Count && _currentInput.Count < _correctCode.Count)
            {
                builder.Append(_previewDialValue);
            }
            else
            {
                builder.Append('-');
            }

            if (i < _correctCode.Count - 1)
            {
                builder.Append(' ');
            }
        }

        _codeText.text = builder.ToString();
    }

    private void OnDestroy()
    {
        _resetTween?.Kill();

        if (_controller != null && !_isSolved)
        {
            _controller.ClearActiveInstance(this);
        }
    }

    public Camera GetCamera()
    {
        if (_controller != null || _controller.PuzzleCamera != null)
        {
            return _controller.PuzzleCamera;
        }
        
        return Camera.main;
    }
}