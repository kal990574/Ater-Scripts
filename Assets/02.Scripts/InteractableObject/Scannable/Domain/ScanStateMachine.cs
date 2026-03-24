using System;
using System.Collections.Generic;

public class ScanStateMachine
{
    private readonly Dictionary<EScannableState, IScanState> _states;
    private readonly Dictionary<EScannableState, Func<IScanState>> _stateFactories;
    private IScanState _currentState;

    public EScannableState CurrentStateType => _currentState == null ? EScannableState.Default : _currentState.StateType;

    public ScanStateMachine(InteractScanObject owner)
    {
        _states = new Dictionary<EScannableState, IScanState>();
        _stateFactories = new Dictionary<EScannableState, Func<IScanState>>
        {
            { EScannableState.Default, () => new ScanDefaultState(owner) },
            { EScannableState.OnProgress, () => new ScanProgressState(owner) },
            { EScannableState.OnReturn, () => new ScanReturnState(owner) },
            { EScannableState.OnHold, () => new ScanHoldState(owner) },
            { EScannableState.OnCompleted, () => new ScanCompleteState(owner) }
        };
    }

    public void ChangeState(EScannableState nextState, bool force = false)
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

    private IScanState CreateState(EScannableState stateType)
    {
        if (_stateFactories.TryGetValue(stateType, out Func<IScanState> factory) == false)
        {
            throw new Exception("[LidarFSM] Invalid state.");
        }

        return factory();
    }
}
