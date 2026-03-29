using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class ScannableQTEInvoker : MonoBehaviour, IQTEInvoker
{
    [Header("Required References")]
    [SerializeField] private ScannableObject _scannableObject;
    [SerializeField] private QTEConfigSOBase _qteConfig;

    [Header("Optional Settings")]
    [SerializeField] private ScanQTESettings _settings;

    [Header("Result Events")]
    [SerializeField] private UnityEvent _onQteSuccess;
    [SerializeField] private UnityEvent _onQteGreatSuccess;
    [SerializeField] private UnityEvent _onQteFail;

    private float _elapsedTime;
    private float _nextTriggerTime;
    private bool _isQteActive;

    public bool IsConfigured => _scannableObject != null && _qteConfig != null && _settings != null;

    private void Awake()
    {
        if (_scannableObject == null)
        {
            _scannableObject = GetComponent<ScannableObject>();
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInParent<ScannableObject>();
        }

        ResetTriggerTimer();
    }

    public void HandleScanning(float deltaTime)
    {
        if (!IsConfigured || _isQteActive)
        {
            return;
        }

        if (_scannableObject.State != EScannableState.OnProgress)
        {
            return;
        }

        _elapsedTime += deltaTime;
        if (_elapsedTime < _nextTriggerTime)
        {
            return;
        }

        TryStartQte();
    }

    public bool HandleScanStopped()
    {
        if (_isQteActive == false)
        {
            ResetTriggerTimer();
            return true;
        }

        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.ForceFailByOwner(this);
        }

        ResetTriggerTimer();
        return false;
    }

    public void ApplyQTEFailure()
    {
        if (_scannableObject == null)
        {
            return;
        }

        _scannableObject.ReduceProgress(_settings.FailPenalty);
        _scannableObject.ChangeState(
            _scannableObject.CurrentProgress <= 0.0f
                ? EScannableState.Default
                : EScannableState.OnReturn);
        _onQteFail?.Invoke();
    }

    public void ApplyQTESuccess()
    {
        if (_scannableObject == null)
        {
            return;
        }

        _scannableObject.ChangeState(
            _scannableObject.IsProgressComplete
                ? EScannableState.OnCompleted
                : EScannableState.OnProgress);
        _onQteSuccess?.Invoke();
    }

    public void ApplyQTEGreatSuccess()
    {
        if (_scannableObject == null)
        {
            return;
        }

        _scannableObject.AddProgress(_settings.GreatSuccessBonus);
        if (_scannableObject.TryTransitToCompleted())
        {
            _onQteGreatSuccess?.Invoke();
            return;
        }

        _scannableObject.ChangeState(EScannableState.OnProgress);
        _onQteGreatSuccess?.Invoke();
    }

    private void TryStartQte()
    {
        if (QTEManager.Instance == null)
        {
            return;
        }

        bool started = QTEManager.Instance.TryPlay(this, _qteConfig, HandleQteEnded);
        if (started == false)
        {
            return;
        }

        _isQteActive = true;
        _scannableObject.PauseScanning();
    }

    private void HandleQteEnded(EQuickTimeEventResult result)
    {
        _isQteActive = false;

        switch (result)
        {
            case EQuickTimeEventResult.Fail:
                ApplyQTEFailure();
                break;
            case EQuickTimeEventResult.Success:
                ApplyQTESuccess();
                break;
            case EQuickTimeEventResult.GreatSuccess:
                ApplyQTEGreatSuccess();
                break;
            default:
                if (_scannableObject != null)
                {
                    _scannableObject.ChangeState(EScannableState.OnProgress);
                }
                break;
        }

        ResetTriggerTimer();
    }

    private void ResetTriggerTimer()
    {
        _elapsedTime = 0.0f;
        if (_settings == null)
        {
            _nextTriggerTime = float.MaxValue;
            return;
        }

        _nextTriggerTime = Random.Range(_settings.MinInterval, _settings.MaxInterval);
    }
}
