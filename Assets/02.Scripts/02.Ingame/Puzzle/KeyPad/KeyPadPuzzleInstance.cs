using DG.Tweening;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class KeyPadPuzzleInstance : MonoBehaviour, IPuzzleIntance
{
    private const int MaxDigitCount = 4;
    private const float SuccessDelay = 0.5f;
    private const float FailDelay = 0.5f;

    private KeyPadController _controller;
    private string _correctCode;
    private bool _isInputLocked;
    private Color _defaultTextColor;
    private Tween _feedbackTween;
    private Tween _shakeTween;
    private Vector3 _defaultShakeLocalPosition;

    [SerializeField] private TextMeshPro _text;
    [SerializeField] private int[] _currentCode = new int[MaxDigitCount];

    [Header("Feedback")]
    [SerializeField] private Color _successTextColor = Color.green;
    [SerializeField] private Color _failTextColor = Color.red;
    [SerializeField] private Transform _shakeTarget;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private Vector3 _shakeStrength = new Vector3(0.03f, 0f, 0f);
    [SerializeField] private int _shakeVibrato = 20;
    [SerializeField] private SoundSFXEmitter _successSfxEmitter;
    [SerializeField] private SoundSFXEmitter _failSfxEmitter;
    [SerializeField] private SoundSFXEmitter _buttonSfxEmitter;
    
    private int _currentLength;

    public void Initialize(KeyPadController controller, string correctCode)
    {
        _controller = controller;
        _correctCode = correctCode;
        _defaultTextColor = _text != null ? _text.color : Color.white;

        Transform shakeTarget = ResolveShakeTarget();
        if (shakeTarget != null)
        {
            _defaultShakeLocalPosition = shakeTarget.localPosition;
        }

        ClearCode();
        RefreshText();
        ResetFeedbackState();
        _isInputLocked = false;
    }

    public void PressKey(int num)
    {
        if (_isInputLocked)
        {
            return;
        }

        if (num >= 0 && num <= 9)
        {
            AppendDigit(num);
            RefreshText();
            _buttonSfxEmitter?.Play();
            return;
        }

        TryEvaluate();
    }

    private void AppendDigit(int num)
    {
        if (_currentLength < MaxDigitCount)
        {
            _currentCode[_currentLength] = num;
            _currentLength++;
            return;
        }

        for (int i = 0; i < MaxDigitCount - 1; i++)
        {
            _currentCode[i] = _currentCode[i + 1];
        }

        _currentCode[MaxDigitCount - 1] = num;
    }

    private void ClearCode()
    {
        _currentLength = 0;

        for (int i = 0; i < _currentCode.Length; i++)
        {
            _currentCode[i] = 0;
        }
    }

    private void RefreshText()
    {
        if (_text == null)
        {
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder(MaxDigitCount);

        for (int i = 0; i < MaxDigitCount; i++)
        {
            if (i < _currentLength)
            {
                builder.Append(_currentCode[i]);
            }
            else
            {
                builder.Append('-');
            }
        }

        _text.text = builder.ToString();
    }

    public void TryEvaluate()
    {
        if (_isInputLocked)
        {
            return;
        }

        string currentCode = GetCurrentCodeString();

        Debug.Log(
            $"[{nameof(KeyPadPuzzleInstance)}] TryEvaluate called on {gameObject.name}. inputCode={currentCode}, expectedCode={_correctCode}",
            this);

        if (_currentLength != MaxDigitCount)
        {
            FailPuzzle();
            return;
        }

        if (currentCode == _correctCode)
        {
            CompletePuzzle();
            return;
        }

        FailPuzzle();
    }

    private string GetCurrentCodeString()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder(MaxDigitCount);

        for (int i = 0; i < _currentLength; i++)
        {
            builder.Append(_currentCode[i]);
        }

        return builder.ToString();
    }

    [ContextMenu("Complete Puzzle")]
    public void CompletePuzzle()
    {
        if (_isInputLocked)
        {
            return;
        }

        _isInputLocked = true;
        ApplyTextColor(_successTextColor);
        _successSfxEmitter?.Play();

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
        if (_isInputLocked)
        {
            return;
        }

        _isInputLocked = true;
        ApplyTextColor(_failTextColor);
        _failSfxEmitter?.Play();
        PlayShake();
        _controller?.HandlePuzzleFail(this);

        KillFeedbackTweenOnly();
        _feedbackTween = DOVirtual.DelayedCall(FailDelay, () =>
        {
            ClearCode();
            RefreshText();
            ResetFeedbackState();
            _isInputLocked = false;
        });
    }

    public void Cancel()
    {
        if (_isInputLocked)
        {
            return;
        }

        _controller?.ClearActiveInstance(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        KillFeedbackTweens();
    }

    private void ApplyTextColor(Color color)
    {
        if (_text == null)
        {
            return;
        }

        _text.color = color;
    }

    private void ResetFeedbackState()
    {
        ApplyTextColor(_defaultTextColor);

        Transform shakeTarget = ResolveShakeTarget();
        if (shakeTarget != null)
        {
            shakeTarget.localPosition = _defaultShakeLocalPosition;
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

    private Transform ResolveShakeTarget()
    {
        if (_shakeTarget != null)
        {
            return _shakeTarget;
        }

        if (_text != null)
        {
            return _text.transform;
        }

        return transform;
    }

    private void KillFeedbackTweens()
    {
        KillFeedbackTweenOnly();

        if (_shakeTween != null && _shakeTween.IsActive())
        {
            _shakeTween.Kill(false);
        }

        _shakeTween = null;
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
