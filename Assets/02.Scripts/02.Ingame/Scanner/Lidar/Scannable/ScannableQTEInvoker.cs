using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class ScannableQTEInvoker : MonoBehaviour, IQTEInvoker
{
    [TabGroup("Inspector", "References")]
    [Required]
    [LabelText("Scannable Object")]
    [SerializeField] private ScannableObject scannableObject;

    [TabGroup("Inspector", "References")]
    [Required]
    [LabelText("QTE Config")]
    [SerializeField] private QTEConfigSOBase _qteConfig;

    [TabGroup("Inspector", "Settings")]
    [Required]
    [LabelText("QTE Settings")]
    [SerializeField] private ScanQTESettings _settings;

    [TabGroup("Inspector", "Events")]
    [LabelText("On QTE Success")]
    [SerializeField] private UnityEvent _onQteSuccess;

    [TabGroup("Inspector", "Events")]
    [LabelText("On QTE Great Success")]
    [SerializeField] private UnityEvent _onQteGreatSuccess;

    [TabGroup("Inspector", "Events")]
    [LabelText("On QTE Failed")]
    [SerializeField] private UnityEvent _onQteFail;

    private float _elapsedTime;
    private float _nextTriggerTime;
    private bool _isQteActive;
    private IScannableQTEHandler _qteHandler => scannableObject != null ? scannableObject.QteHandler : null;

    public GameObject Owner => gameObject;
    public bool IsConfigured => scannableObject != null && _qteConfig != null && _settings != null;
    public event Action OnQteSucceeded;
    public event Action OnQteGreatSucceeded;
    public event Action OnQteFailed;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Is Configured")]
    private bool DebugIsConfigured => IsConfigured;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Is QTE Active")]
    private bool DebugIsQteActive => _isQteActive;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Elapsed Time")]
    private float DebugElapsedTime => _elapsedTime;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Next Trigger Time")]
    private float DebugNextTriggerTime => _nextTriggerTime;

    private void Awake()
    {
        ResolveReferences();

        ResetTriggerTimer();
    }

    public void HandleScanning(float deltaTime)
    {
        if (!IsConfigured || _isQteActive)
        {
            return;
        }

        if (_qteHandler == null || _qteHandler.State != EScanState.OnProgress)
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
            return true;
        }

        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.ForceFailByOwner(this);
        }
        
        return false;
    }

    public void ApplyQTEFailure()
    {
        if (_qteHandler == null)
        {
            return;
        }

        OnQteFailed?.Invoke();
        _qteHandler.HandleQteFailure(_settings.FailPenalty);
        _onQteFail?.Invoke();
    }

    public void ApplyQTESuccess()
    {
        if (_qteHandler == null)
        {
            return;
        }

        OnQteSucceeded?.Invoke();
        _qteHandler.HandleQteSuccess();
        _onQteSuccess?.Invoke();
    }

    public void ApplyQTEGreatSuccess()
    {
        if (_qteHandler == null)
        {
            return;
        }

        OnQteGreatSucceeded?.Invoke();
        _qteHandler.HandleQteGreatSuccess(_settings.GreatSuccessBonus);
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
        _qteHandler?.PauseScanning();
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
                _qteHandler?.ResumeScanningAfterQte();
                break;
        }

        ResetTriggerTimer();
    }

    private void ResolveReferences()
    {
        if (scannableObject == null)
        {
            scannableObject = GetComponent<ScannableObject>();
        }

        if (scannableObject == null)
        {
            scannableObject = GetComponentInParent<ScannableObject>();
        }
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
