using _02.Scripts.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class QTEManager : MonoBehaviour
{
    //TOdo 현재는 타이밍 퀵타임 이벤트만 제작됨.
    //이후엔 더 추가할것
    public static QTEManager Instance { get; private set; }

    [SerializeField]private IPlayerInput _input;
    
    //모든 스킬체크 UI 보관 추후 lazyActive로 바뀔수 있음
    [SerializeField] private CircleTimingQTEUI _circleTimingQteUi;
    
    //QTE 세팅도 요청시 받아오도록 추후 수정
    public TimingQuickTimeEventConfig QteConfig;
    
    private IQuickTimeEvent _currentEvent;
    private IQTEInvoker _currentOwner;
    private Action<EQuickTimeEventResult> _onEnded;
    
    
    public ITimingQuickTimeEventView TimingQuickTimeEventView => _circleTimingQteUi;
    
    public bool IsPlaying
    {
        get
        {
            return _currentEvent != null && _currentEvent.IsPlaying == true;
        }
    }

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
        
        _circleTimingQteUi.Hide();
    }

    private void Update()
    {
        if (_currentEvent == null)
        {
            return;
        }

        if (_currentEvent.IsPlaying == true)
        {
            _currentEvent.Tick(Time.deltaTime);
        }

        if (_currentEvent.IsFinished == true)
        {
            EndCurrent(_currentEvent.Result);
        }
    }

    public bool TryPlay(IQTEInvoker owner,  Action<EQuickTimeEventResult> onEnded)
    {
        if (owner == null)
        {
            return false;
        }

        if (IsPlaying == true)
        {
            return false;
        }

        _currentOwner = owner;
        _currentEvent = new TimingQTERunner(QteConfig, TimingQuickTimeEventView);
        _onEnded = onEnded;

        _currentEvent.Begin();
        return true;
    }

    public void SubmitCurrent()
    {
        if (_currentEvent == null)
        {
            return;
        }

        if (_currentEvent.IsPlaying == false)
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

        if (_currentEvent.IsFinished == true)
        {
            EndCurrent(_currentEvent.Result);
        }
    }
    
    public void ForceFailByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null)
        {
            return;
        }

        if (ReferenceEquals(_currentOwner, owner) == false)
        {
            return;
        }

        if (_currentEvent.IsPlaying == true)
        {
            _currentEvent.Cancel();
        }

        EndCurrent(EQuickTimeEventResult.Fail);
    }

    public void CancelByOwner(IQTEInvoker owner)
    {
        if (_currentEvent == null)
        {
            return;
        }

        if (ReferenceEquals(_currentOwner, owner) == false)
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