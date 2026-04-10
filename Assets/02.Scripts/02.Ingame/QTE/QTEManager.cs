using System;
using UnityEngine;

[DisallowMultipleComponent]
public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    [Header("Required References")]
    [SerializeField] private UI_CircleTimingQTE _uiCircleTimingQteUi;

    private IQuickTimeEvent _currentEvent;
    private IQTEInvoker _currentOwner;
    private GameEventPublisher _eventPublisher;
    private Action<EQuickTimeEventResult> _onEnded;

    public ITimingQuickTimeEventView TimingQuickTimeEventView => _uiCircleTimingQteUi;
    public bool IsPlaying => _currentEvent != null && _currentEvent.IsPlaying;
    public IQTEInvoker CurrentOwner => _currentOwner;
    public IQuickTimeEvent CurrentEvent => _currentEvent;
    public ISoundService SoundService => SoundManager.Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_uiCircleTimingQteUi == null)
        {
            Debug.LogError($"[{nameof(QTEManager)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        _uiCircleTimingQteUi.Hide();
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
            EndCurrent(_currentEvent.QTEType, _currentEvent.Result);
        }
    }

    public bool TryPlay(IQTEInvoker owner, QTEConfigSOBase config, Action<EQuickTimeEventResult> onEnded)
    {
        if (owner == null || config == null || IsPlaying)
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
            EndCurrent(_currentEvent.QTEType, _currentEvent.Result);
        }
    }

    public void ForceFailByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null || ReferenceEquals(_currentOwner, owner) == false)
        {
            return;
        }

        EQTEType qteType = _currentEvent.QTEType;

        if (_currentEvent.IsPlaying)
        {
            _currentEvent.Cancel();
        }

        EndCurrent(qteType, EQuickTimeEventResult.Fail);
    }

    public void CancelByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null || ReferenceEquals(_currentOwner, owner) == false)
        {
            return;
        }

        CancelCurrent();
    }

    private void EndCurrent(EQTEType qteType, EQuickTimeEventResult result)
    {
        Action<EQuickTimeEventResult> callback = _onEnded;

        _eventPublisher.TryPublish(context => new QteRawEvent(
            context,
            _currentOwner.Owner,
                qteType,
                result));

        _currentEvent = null;
        _currentOwner = null;
        _onEnded = null;

        callback?.Invoke(result);
    }

    private IQuickTimeEvent CreateEvent(QTEConfigSOBase config)
    {
        switch (config.QTEType)
        {
            case EQTEType.Timing:
            {
                if (config is TimingQuickTimeEventConfig timingConfig == false)
                {
                    Debug.LogError(
                        $"[{nameof(QTEManager)}] Config type mismatch. QTEType={config.QTEType}, Config={config.GetType().Name}",
                        this);
                    return null;
                }

                return new TimingQTERunner(this, timingConfig, TimingQuickTimeEventView);
            }

            default:
            {
                Debug.LogError(
                    $"[{nameof(QTEManager)}] Unsupported QTE type: {config.QTEType}",
                    this);
                return null;
            }
        }
    }
}