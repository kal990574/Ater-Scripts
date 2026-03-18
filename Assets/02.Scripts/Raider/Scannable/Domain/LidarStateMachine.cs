using System;
using System.Collections.Generic;
using UnityEngine;

public class LidarStateMachine
{
    private readonly LidarTarget _owner;
    private readonly Dictionary<ELidarTargetState, ILidarTargetState> _states;
    private ILidarTargetState _currentState;

    public ELidarTargetState CurrentStateType => _currentState == null ? ELidarTargetState.Default : _currentState.StateType;

    public LidarStateMachine(LidarTarget owner)
    {
        _owner = owner;
        _states = new Dictionary<ELidarTargetState, ILidarTargetState>();
    }

    public void ChangeState(ELidarTargetState nextState, bool force = false)
    {
        if (force == false && _currentState != null && _currentState.StateType == nextState)
        {
            return;
        }

        if (_states.ContainsKey(nextState) == false)
        {
            _states[nextState] = CreateState(nextState);
        }
        
        _currentState?.Exit();
        _currentState = _states[nextState];
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
        switch (stateType)
        {
            case ELidarTargetState.Default:
            {
                return new LidarDefaultState(_owner);
            }
            case ELidarTargetState.OnProgress:
            {
                return new LidarProgressState(_owner);
            }
            case ELidarTargetState.OnReturn:
            {
                return new LidarReturnState(_owner);
            }
            case ELidarTargetState.OnMinigame:
            {
                return new LidarMinigameState(_owner);
            }
            default:
                {
                    throw new Exception("[LidarFSM] 작성하지 않은 스테이트");
                }
        }
    }
}