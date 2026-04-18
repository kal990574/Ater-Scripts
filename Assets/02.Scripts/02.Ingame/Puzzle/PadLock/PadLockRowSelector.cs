using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PadLockRowSelector : MonoBehaviour, IPointerClickHandler
{
    public enum RowIndex
    {
        Row1,
        Row2,
        Row3,
        Row4
    }

    [Header("Target Row")]
    [SerializeField] private RowIndex _rowIndex = RowIndex.Row1;

    [Header("Number Range")]
    [SerializeField] private int _startValue = 1;
    [SerializeField] private int _minValue = 1;
    [SerializeField] private int _maxValue = 9;

    [Header("Visual")]
    [SerializeField] private Vector3 _rotationAxis = new(0f, 0f, 1f);
    [SerializeField] private float _rotationStep = 40f;
    [SerializeField] private int _resetExtraSpins = 1;
    [SerializeField] private Ease _resetEase = Ease.OutCubic;

    private PadLockPuzzleInstance _puzzleInstance;
    private int _currentValue;
    private Quaternion _defaultLocalRotation;
    private Tween _resetTween;

    private void Awake()
    {
        _defaultLocalRotation = transform.localRotation;
        _currentValue = Mathf.Clamp(_startValue, _minValue, _maxValue);
        ApplyVisualFromValue(_currentValue);
    }

    public void Bind(PadLockPuzzleInstance puzzleInstance)
    {
        _puzzleInstance = puzzleInstance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_puzzleInstance == null)
        {
            return;
        }

        if (!_puzzleInstance.CanAcceptInput())
        {
            return;
        }

        RotateNext();
        _puzzleInstance.SetRowValue(_rowIndex, _currentValue);
    }

    public void ResetToDefault()
    {
        KillResetTween();
        SetValue(_startValue);
    }

    public void AnimateResetToDefault(float duration)
    {
        KillResetTween();

        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = GetRotationForValue(_startValue);

        Vector3 rotationAxis = _rotationAxis.sqrMagnitude > 0f ? _rotationAxis.normalized : Vector3.forward;
        float rotationDelta = GetPositiveRotationDelta(startRotation, targetRotation, rotationAxis);
        float totalRotation = (_resetExtraSpins * 360f) + rotationDelta;

        _currentValue = _startValue;

        _resetTween = DOTween.To(
                () => 0f,
                angle => transform.localRotation = startRotation * Quaternion.AngleAxis(angle, rotationAxis),
                totalRotation,
                duration)
            .SetEase(_resetEase)
            .OnKill(() => transform.localRotation = targetRotation)
            .OnComplete(() => transform.localRotation = targetRotation);
    }

    public void SetValue(int value)
    {
        KillResetTween();
        _currentValue = value;
        ApplyVisualFromValue(value);
    }

    private void RotateNext()
    {
        _currentValue++;
        if (_currentValue > _maxValue)
        {
            _currentValue = _minValue;
        }

        transform.Rotate(_rotationAxis.normalized, _rotationStep, Space.Self);
    }

    private void ApplyVisualFromValue(int value)
    {
        transform.localRotation = GetRotationForValue(value);
    }

    private Quaternion GetRotationForValue(int value)
    {
        float offsetFromStart = value - _startValue;
        Quaternion stepRotation = Quaternion.AngleAxis(_rotationStep * offsetFromStart, _rotationAxis.normalized);
        return _defaultLocalRotation * stepRotation;
    }

    private static float GetPositiveRotationDelta(Quaternion startRotation, Quaternion targetRotation, Vector3 axis)
    {
        Quaternion deltaRotation = Quaternion.Inverse(startRotation) * targetRotation;
        deltaRotation.ToAngleAxis(out float angle, out Vector3 deltaAxis);

        if (float.IsNaN(angle))
        {
            return 0f;
        }

        if (Vector3.Dot(deltaAxis, axis) < 0f)
        {
            angle = -angle;
        }

        while (angle < 0f)
        {
            angle += 360f;
        }

        return angle;
    }

    private void KillResetTween()
    {
        if (_resetTween != null && _resetTween.IsActive())
        {
            _resetTween.Kill(false);
        }

        _resetTween = null;
    }

    private void OnDestroy()
    {
        KillResetTween();
    }
}
