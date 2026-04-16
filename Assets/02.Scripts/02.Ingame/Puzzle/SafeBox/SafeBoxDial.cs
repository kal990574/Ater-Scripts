using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SafeBoxDial : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private const int MinValue = 0;
    private const int MaxValue = 15;
    private const int ValueCount = 16;

    [Header("Rotation")]
    [SerializeField] private float _degreesPerStep = -22.5f;
    [SerializeField] private Vector3 _rotationAxis = new(0f, 0f, -1f);

    [Header("Drag")]
    [SerializeField] private Camera _eventCamera;
    [SerializeField] private Transform _dialCenter;
    [SerializeField] private float _minDragRadius = 20f;

    [Header("State")]
    [SerializeField] private int _currentValue;
    [SerializeField] private float _currentVisualAngle;

    private SafeBoxInstance _owner;
    private bool _isDragging;
    private float _previousPointerAngle;
    private Tween _rotationTween;

    public int CurrentValue => _currentValue;

    public void Bind(SafeBoxInstance owner)
    {
        _owner = owner;
        _eventCamera = owner.GetCamera();
        ForceResetVisualImmediate();
        _owner?.SetPreviewDialValue(_currentValue);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_owner == null || _owner.IsBusy || _owner.IsInputFull)
        {
            return;
        }

        if (!TryGetPointerAngle(eventData, out float pointerAngle))
        {
            return;
        }

        _rotationTween?.Kill();
        _isDragging = true;
        _previousPointerAngle = pointerAngle;
        _owner.SetPreviewDialValue(_currentValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        if (_owner == null || _owner.IsBusy || _owner.IsInputFull)
        {
            return;
        }

        if (!TryGetPointerAngle(eventData, out float currentPointerAngle))
        {
            return;
        }

        float deltaAngle = Mathf.DeltaAngle(_previousPointerAngle, currentPointerAngle);
        _previousPointerAngle = currentPointerAngle;

        _currentVisualAngle += deltaAngle;
        ApplyVisualAngle(_currentVisualAngle);
        UpdateCurrentValueFromAngle();

        _owner.SetPreviewDialValue(_currentValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;

        if (_owner == null || !_owner.CanAcceptDialCommit())
        {
            return;
        }

        SnapToNearestStep();
        _owner.SetPreviewDialValue(_currentValue);
        _owner.CommitDialValue(_currentValue);
    }

    public Tween PlayResetAnimation(float duration, Ease ease, TweenCallback onComplete)
    {
        _rotationTween?.Kill();

        _currentValue = 0;

        _rotationTween = DOTween.To(
                () => _currentVisualAngle,
                value =>
                {
                    _currentVisualAngle = value;
                    ApplyVisualAngle(_currentVisualAngle);
                    UpdateCurrentValueFromAngle();

                    if (_owner != null && !_owner.IsInputFull)
                    {
                        _owner.SetPreviewDialValue(_currentValue);
                    }
                },
                0f,
                duration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                _currentVisualAngle = 0f;
                ApplyVisualAngle(_currentVisualAngle);
                UpdateCurrentValueFromAngle();

                if (_owner != null && !_owner.IsInputFull)
                {
                    _owner.SetPreviewDialValue(_currentValue);
                }

                onComplete?.Invoke();
            });

        return _rotationTween;
    }

    public void ForceResetVisualImmediate()
    {
        _rotationTween?.Kill();

        _isDragging = false;
        _currentValue = 0;
        _currentVisualAngle = 0f;

        ApplyVisualAngle(_currentVisualAngle);
    }

    private bool TryGetPointerAngle(PointerEventData eventData, out float angle)
    {
        angle = 0f;

        Camera cameraToUse = ResolveEventCamera(eventData);
        Transform centerTransform = ResolveDialCenter();
        if (centerTransform == null)
        {
            return false;
        }

        Vector2 centerScreenPosition = RectTransformUtility.WorldToScreenPoint(cameraToUse, centerTransform.position);
        Vector2 direction = eventData.position - centerScreenPosition;

        if (direction.sqrMagnitude < _minDragRadius * _minDragRadius)
        {
            return false;
        }

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return true;
    }

    private Camera ResolveEventCamera(PointerEventData eventData)
    {
        if (_eventCamera != null)
        {
            return _eventCamera;
        }

        if (eventData.pressEventCamera != null)
        {
            return eventData.pressEventCamera;
        }

        if (eventData.enterEventCamera != null)
        {
            return eventData.enterEventCamera;
        }

        return Camera.main;
    }

    private Transform ResolveDialCenter()
    {
        if (_dialCenter != null)
        {
            return _dialCenter;
        }

        return transform;
    }

    private void SnapToNearestStep()
    {
        float stepIndex = Mathf.Round(-_currentVisualAngle / _degreesPerStep);
        float normalizedStepIndex = Mod(stepIndex, ValueCount);

        _currentValue = (int)normalizedStepIndex;
        _currentVisualAngle = -_currentValue * _degreesPerStep;
        ApplyVisualAngle(_currentVisualAngle);
    }

    private void UpdateCurrentValueFromAngle()
    {
        float stepIndex = Mathf.Round(-_currentVisualAngle / _degreesPerStep);
        float normalizedStepIndex = Mod(stepIndex, ValueCount);
        _currentValue = Mathf.Clamp((int)normalizedStepIndex, MinValue, MaxValue);
    }

    private void ApplyVisualAngle(float angle)
    {
        transform.localRotation = Quaternion.AngleAxis(angle, _rotationAxis.normalized);
    }

    private static float Mod(float value, float modulus)
    {
        float result = value % modulus;
        if (result < 0f)
        {
            result += modulus;
        }

        return result;
    }
}