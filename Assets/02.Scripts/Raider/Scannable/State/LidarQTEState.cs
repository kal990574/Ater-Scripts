public class LidarQTEState : LidarTargetStateBase
{
    private bool _hasResult;
    private EQuickTimeEventResult _result;

    public override ELidarTargetState StateType => ELidarTargetState.OnPlayQTE;

    public LidarQTEState(LidarTarget owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _hasResult = false;
        _result = EQuickTimeEventResult.Default;

        if (QTEManager.Instance == null)
        {
            Owner.ChangeState(ELidarTargetState.OnProgress);
            return;
        }

        bool started = QTEManager.Instance.TryPlay(Owner, HandleQTEResult);
        if (started == false)
        {
            Owner.ChangeState(ELidarTargetState.OnProgress);
        }
    }

    public override void Exit()
    {
        Owner.SetQTEDelay();
    }

    public override void Tick(float deltaTime)
    {
        if (_hasResult == false)
        {
            return;
        }

        ApplyResultAndTransit();
    }

    public override void OnScanLost()
    {
        if (QTEManager.Instance == null)
        {
            _result = EQuickTimeEventResult.Fail;
            _hasResult = true;
            return;
        }

        QTEManager.Instance.ForceFailByOwner(Owner);
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
                Owner.ChangeState(ELidarTargetState.OnProgress);
                break;
            case EQuickTimeEventResult.Fail:
                Owner.HandleQteFailure();
                break;
            case EQuickTimeEventResult.Success:
                Owner.TransitToProgressOrCompleted();
                break;
            case EQuickTimeEventResult.GreatSuccess:
                Owner.ApplyGreatSuccessBonus();
                Owner.TransitToProgressOrCompleted();
                break;
        }
    }
}
