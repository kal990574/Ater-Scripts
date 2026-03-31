using System;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class QTEManager : GameEventPublisher
{
    public static QTEManager Instance { get; private set; }

    [Header("Required References")]
    [SerializeField] private UI_CircleTimingQTE uiCircleTimingQteUi;
    [FormerlySerializedAs("QteConfig")]
    [SerializeField] private TimingQuickTimeEventConfig _qteConfig;

    private IQuickTimeEvent _currentEvent;
    private IQTEInvoker _currentOwner;
    private Action<EQuickTimeEventResult> _onEnded;

    public ITimingQuickTimeEventView TimingQuickTimeEventView => uiCircleTimingQteUi;
    public bool IsPlaying => _currentEvent != null && _currentEvent.IsPlaying;
    public IQTEInvoker CurrentOwner => _currentOwner;
    public IQuickTimeEvent CurrentEvent => _currentEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (uiCircleTimingQteUi == null || _qteConfig == null)
        {
            Debug.LogError($"[{nameof(QTEManager)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        uiCircleTimingQteUi.Hide();
    }

    private void Update()
    {
        if (_currentEvent == null)
        {
            return;
        }

        if (_currentEvent.IsPlaying)
        {
            _currentEvent.Tick(Time.deltaTime);
        }

        if (_currentEvent.IsFinished)
        {
            EndCurrent(_currentEvent.Result);
        }
    }

    public bool TryPlay(IQTEInvoker owner, Action<EQuickTimeEventResult> onEnded)
    {
        return TryPlay(owner, _qteConfig, onEnded);
    }

    public bool TryPlay(IQTEInvoker owner, QTEConfigSOBase config, Action<EQuickTimeEventResult> onEnded)
    {
        if (owner == null || IsPlaying)
        {
            return false;
        }

        _currentOwner = owner;
        _currentEvent = CreateEvent(config);
        if (_currentEvent == null)
        {
            _currentOwner = null;
            return false;
        }

        _onEnded = onEnded;

        _currentEvent.Begin();
        return true;
    }

    public void SubmitCurrent()
    {
        if (_currentEvent == null || _currentEvent.IsPlaying == false)
        {
            return;
        }

        _currentEvent.Submit();
    }

    public void CancelCurrent()
    {
        if (_currentEvent == null)
        {
            return;
        }

        _currentEvent.Cancel();
        if (_currentEvent.IsFinished)
        {
            EndCurrent(_currentEvent.Result);
        }
    }

    public void ForceFailByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null || ReferenceEquals(_currentOwner, owner) == false)
        {
            return;
        }

        if (_currentEvent.IsPlaying)
        {
            _currentEvent.Cancel();
        }

        EndCurrent(EQuickTimeEventResult.Fail);
    }

    public void CancelByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null || ReferenceEquals(_currentOwner, owner) == false)
        {
            return;
        }

        CancelCurrent();
    }

    private void EndCurrent(EQuickTimeEventResult result)
    {
        Action<EQuickTimeEventResult> callback = _onEnded;

        _currentEvent = null;
        _currentOwner = null;
        _onEnded = null;
        callback?.Invoke(result);
        
        if (TryGetHub(out GameEventHub hub) == false)
        {
            Debug.LogWarning("[PickupEventEmitter] GameEventHub가 존재하지 않습니다.");
            return;
        }
        GameEventContext eventContext = CreateContext();
        switch (result)
        {
            case EQuickTimeEventResult.Fail:
                QTEFailEvent failEvent = new QTEFailEvent(eventContext);
                hub.Publish(in failEvent);
                break;
            case EQuickTimeEventResult.Success:
                QTEGoodEvent goodEvent = new QTEGoodEvent(eventContext);
                hub.Publish(in goodEvent);
                break;
            case EQuickTimeEventResult.GreatSuccess:
                QTEGreatEvent greatEvent = new QTEGreatEvent(eventContext);
                hub.Publish(in greatEvent);
                break;
        }

        
    }

    private IQuickTimeEvent CreateEvent(QTEConfigSOBase config)
    {
        if (config == null)
        {
            Debug.LogError($"[{nameof(QTEManager)}] QTE config is missing.", this);
            return null;
        }

        if (config is TimingQuickTimeEventConfig timingConfig)
        {
            return new TimingQTERunner(timingConfig, TimingQuickTimeEventView);
        }

        Debug.LogError($"[{nameof(QTEManager)}] Unsupported QTE config type: {config.GetType().Name}", this);
        return null;
    }
}
