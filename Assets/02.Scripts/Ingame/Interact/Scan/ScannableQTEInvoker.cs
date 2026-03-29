using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class ScannableQTEInvoker : MonoBehaviour, IQTEInvoker
{
    [Header("Required References")]
    [SerializeField] private ScannableObject scannableObject;
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

    public bool IsConfigured => scannableObject != null && _qteConfig != null && _settings != null;

    private void Awake()
    {
        if (scannableObject == null)
        {
            scannableObject = GetComponent<ScannableObject>();
        }

        if (scannableObject == null)
        {
            scannableObject = GetComponentInParent<ScannableObject>();
        }

        ResetTriggerTimer();
    }

    public void HandleScanning(float deltaTime)
    {
        if (!IsConfigured || _isQteActive)
        {
            return;
        }

        if (scannableObject.State != EScanState.OnProgress)
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
        if (scannableObject == null)
        {
            return;
        }

        scannableObject.ReduceProgress(_settings.FailPenalty);
        scannableObject.ChangeState(
            scannableObject.CurrentProgress <= 0.0f
                ? EScanState.Default
                : EScanState.OnReturn);
        _onQteFail?.Invoke();
    }

    public void ApplyQTESuccess()
    {
        if (scannableObject == null)
        {
            return;
        }

        scannableObject.ChangeState(
            scannableObject.IsProgressComplete
                ? EScanState.OnCompleted
                : EScanState.OnProgress);
        _onQteSuccess?.Invoke();
    }

    public void ApplyQTEGreatSuccess()
    {
        if (scannableObject == null)
        {
            return;
        }

        scannableObject.AddProgress(_settings.GreatSuccessBonus);
        if (scannableObject.TryTransitToCompleted())
        {
            _onQteGreatSuccess?.Invoke();
            return;
        }

        scannableObject.ChangeState(EScanState.OnProgress);
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
        scannableObject.PauseScanning();
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
                if (scannableObject != null)
                {
                    scannableObject.ChangeState(EScanState.OnProgress);
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
