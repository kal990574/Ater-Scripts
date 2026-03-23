using System;
using System.Collections.Generic;

public class LidarStateMachine
{
    private readonly Dictionary<ELidarTargetState, ILidarTargetState> _states;
    private readonly Dictionary<ELidarTargetState, Func<ILidarTargetState>> _stateFactories;
    private ILidarTargetState _currentState;

    public ELidarTargetState CurrentStateType => _currentState == null ? ELidarTargetState.Default : _currentState.StateType;

    public LidarStateMachine(LidarTarget owner)
    {
        _states = new Dictionary<ELidarTargetState, ILidarTargetState>();
        _stateFactories = new Dictionary<ELidarTargetState, Func<ILidarTargetState>>
        {
            { ELidarTargetState.Default, () => new LidarDefaultState(owner) },
            { ELidarTargetState.OnProgress, () => new LidarProgressState(owner) },
            { ELidarTargetState.OnReturn, () => new LidarReturnState(owner) },
            { ELidarTargetState.OnPlayQTE, () => new LidarQTEState(owner) },
            { ELidarTargetState.OnCompleted, () => new LidarCompleteState(owner) }
        };
    }

    public void ChangeState(ELidarTargetState nextState, bool force = false)
    {
        if (force == false && _currentState != null && _currentState.StateType == nextState)
        {
            return;
        }

        if (_states.TryGetValue(nextState, out ILidarTargetState next) == false)
        {
            next = CreateState(nextState);
            _states[nextState] = next;
        }

        _currentState?.Exit();
        _currentState = next;
        _currentState.Enter();
    }

    public void Tick(float deltaTime)
    {
        _currentState?.Tick(deltaTime);
    }

    public void OnScanning(float deltaTime)
    {
        _currentState?.OnScanning(deltaTime);
    }

    public void OnScanLost()
    {
        _currentState?.OnScanLost();
    }

    private ILidarTargetState CreateState(ELidarTargetState stateType)
    {
        if (_stateFactories.TryGetValue(stateType, out Func<ILidarTargetState> factory) == false)
        {
            throw new Exception("[LidarFSM] 작성하지 않은 스테이트");
        }

        return factory();
    }
}
