public class LidarQTEState : ILidarTargetState
{
    private readonly LidarTarget _owner;
    
    private bool _hasResult;
    private EQuickTimeEventResult _result;

    public ELidarTargetState StateType => ELidarTargetState.OnPlayQTE;

    public LidarQTEState(LidarTarget owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
        _hasResult = false;
        _result = EQuickTimeEventResult.Default;
        
        if (QTEManager.Instance == null)
        {
            _owner.ChangeState(ELidarTargetState.OnProgress);
            return;
        }

        bool started = QTEManager.Instance.TryPlay(_owner, HandleQTEResult);
       
        if (started == false)
        {
            _owner.ChangeState(ELidarTargetState.OnProgress);
        }
        
    }

    public void Exit()
    {
        _owner.SetQTEDelay();
    }

    public void Tick(float deltaTime)
    {
        if (_hasResult == false)
        {
            return;
        }

        ApplyResultAndTransit();
    }

    public void OnScanning(float deltaTime)
    {
    }

    public void OnScanLost()
    {
        if (QTEManager.Instance == null)
        {
            _result = EQuickTimeEventResult.Fail;
            _hasResult = true;
            return;
        }

        QTEManager.Instance.ForceFailByOwner(_owner);
    }

    private void HandleQTEResult(EQuickTimeEventResult result)
    {
        _result = result;
        _hasResult = true;
    }

    private void ApplyResultAndTransit()
    {
        _hasResult = false;

        switch (_result)
        {
            case EQuickTimeEventResult.Default:
            {
                _owner.ChangeState(ELidarTargetState.OnProgress);
                break;
            }
            case EQuickTimeEventResult.Fail:
            {
                _owner.ReduceProgress(_owner.Settings.FailPenalty);

                if (_owner.CurrentProgress <= 0.0f)
                {
                    _owner.ChangeState(ELidarTargetState.Default);
                }
                else
                {
                    _owner.ChangeState(ELidarTargetState.OnReturn);
                }

                break;
            }
            case EQuickTimeEventResult.Success:
            {
                if (_owner.IsProgressComplete == true)
                {
                    _owner.ChangeState(ELidarTargetState.OnCompleted);
                }
                else
                {
                    _owner.ChangeState(ELidarTargetState.OnProgress);
                }

                break;
            }
            case EQuickTimeEventResult.GreatSuccess:
            {
                _owner.AddProgress(_owner.Settings.GreatSuccessBonus);

                if (_owner.IsProgressComplete == true)
                {
                    _owner.ChangeState(ELidarTargetState.OnCompleted);
                }
                else
                {
                    _owner.ChangeState(ELidarTargetState.OnProgress);
                }

                break;
            }
        }
    }
}