using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class KeyPadPuzzleInstance : MonoBehaviour
{
    private const int MaxDigitCount = 4;

    private KeyPadController _controller;
    private string _correctCode;

    [SerializeField] private TextMeshPro _text;
    [SerializeField] private int[] _currentCode = new int[MaxDigitCount];

    private int _currentLength;

    public void Initialize(KeyPadController controller, string correctCode)
    {
        _controller = controller;
        _correctCode = correctCode;

        ClearCode();
        RefreshText();
    }

    public void PressKey(int num)
    {
        _controller.ButtonClick();
        
        if (num >= 0 && num <= 9)
        {
            AppendDigit(num);
            RefreshText();
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
        _controller?.HandlePuzzleSuccess(this);
        Destroy(gameObject);
    }

    [ContextMenu("Fail Puzzle")]
    public void FailPuzzle()
    {
        _controller?.HandlePuzzleFail(this);

        ClearCode();
        RefreshText();
    }

    public void Cancel()
    {
        _controller?.ClearActiveInstance(this);
        Destroy(gameObject);
    }
}