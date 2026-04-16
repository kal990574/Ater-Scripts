using System;
using System.Collections.Generic;

public class ScanFSM
{
    private readonly Dictionary<EScanState, IScanState> _states;
    private readonly Dictionary<EScanState, Func<IScanState>> _stateFactories;
    private IScanState _currentState;

    public EScanState CurrentStateType => _currentState == null ? EScanState.Default : _currentState.StateType;

    public ScanFSM(IScanStateContext owner)
    {
        _states = new Dictionary<EScanState, IScanState>();
        _stateFactories = new Dictionary<EScanState, Func<IScanState>>
        {
            { EScanState.Default, () => new ScanDefaultState(owner) },
            { EScanState.OnProgress, () => new ScanProgressState(owner) },
            { EScanState.OnReturn, () => new ScanReturnState(owner) },
            { EScanState.OnHold, () => new ScanHoldState(owner) },
            { EScanState.OnCompleted, () => new ScanCompleteState(owner) }
        };
    }

    public void ChangeState(EScanState nextState, bool force = false)
    {
        if (force == false && _currentState != null && _currentState.StateType == nextState)
        {
            return;
        }

        if (_states.TryGetValue(nextState, out IScanState next) == false)
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

    public void OnScanStopped()
    {
        _currentState?.OnScanLost();
    }

    private IScanState CreateState(EScanState stateType)
    {
        if (_stateFactories.TryGetValue(stateType, out Func<IScanState> factory) == false)
        {
            throw new Exception("[LidarFSM] Invalid state.");
        }

        return factory();
    }
}
