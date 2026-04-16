using System.Collections.Generic;
using UnityEngine;

public class SafeBoxStateMachine
{
    private readonly List<int> _correctCode = new();
    private readonly List<int> _currentInput = new();

    public IReadOnlyList<int> CurrentInput => _currentInput;
    public int TargetInputCount => _correctCode.Count;
    public int PreviewDialValue { get; private set; }
    public bool IsSolved { get; private set; }
    public bool IsResetAnimating { get; private set; }
    public bool IsInputFull => _currentInput.Count >= _correctCode.Count;
    public bool IsBusy => IsSolved || IsResetAnimating;

    public void Initialize(IReadOnlyList<int> correctCode)
    {
        _correctCode.Clear();
        if (correctCode != null)
        {
            for (int i = 0; i < correctCode.Count; i++)
            {
                _correctCode.Add(Mathf.Clamp(correctCode[i], 0, 15));
            }
        }

        _currentInput.Clear();
        PreviewDialValue = 0;
        IsSolved = false;
        IsResetAnimating = false;
    }

    public bool CanAcceptPreview()
    {
        if (IsSolved || IsResetAnimating)
        {
            return false;
        }

        if (_correctCode.Count == 0)
        {
            return false;
        }

        return _currentInput.Count < _correctCode.Count;
    }

    public bool TrySetPreview(int value)
    {
        if (!CanAcceptPreview())
        {
            return false;
        }

        PreviewDialValue = Mathf.Clamp(value, 0, 15);
        return true;
    }

    public bool CanAcceptCommit()
    {
        return CanAcceptPreview();
    }

    public bool TryCommit(int value)
    {
        if (!CanAcceptCommit())
        {
            return false;
        }

        _currentInput.Add(Mathf.Clamp(value, 0, 15));
        PreviewDialValue = 0;
        return true;
    }

    public SafeBoxEvaluationResult Evaluate()
    {
        if (IsSolved || IsResetAnimating || _correctCode.Count == 0)
        {
            return SafeBoxEvaluationResult.Blocked;
        }

        if (_currentInput.Count != _correctCode.Count)
        {
            return SafeBoxEvaluationResult.Fail;
        }

        for (int i = 0; i < _correctCode.Count; i++)
        {
            if (_currentInput[i] != _correctCode[i])
            {
                return SafeBoxEvaluationResult.Fail;
            }
        }

        return SafeBoxEvaluationResult.Success;
    }

    public void MarkSolved()
    {
        IsSolved = true;
    }

    public void Reset(bool animated)
    {
        _currentInput.Clear();
        PreviewDialValue = 0;
        IsResetAnimating = animated;
    }

    public void FinishResetAnimation()
    {
        IsResetAnimating = false;
        PreviewDialValue = 0;
    }
}
