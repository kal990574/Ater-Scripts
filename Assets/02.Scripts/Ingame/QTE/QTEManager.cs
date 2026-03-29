using System;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    [Header("Required References")]
    [SerializeField] private CircleTimingQTEUI _circleTimingQteUi;
    [FormerlySerializedAs("QteConfig")]
    [SerializeField] private TimingQuickTimeEventConfig _qteConfig;

    private IQuickTimeEvent _currentEvent;
    private IQTEInvoker _currentOwner;
    private Action<EQuickTimeEventResult> _onEnded;

    public ITimingQuickTimeEventView TimingQuickTimeEventView => _circleTimingQteUi;
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

        if (_circleTimingQteUi == null || _qteConfig == null)
        {
            Debug.LogError($"[{nameof(QTEManager)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        _circleTimingQteUi.Hide();
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
        if (owner == null || IsPlaying)
        {
            return false;
        }

        _currentOwner = owner;
        _currentEvent = new TimingQTERunner(_qteConfig, TimingQuickTimeEventView);
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
    }
}
