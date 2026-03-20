using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LidarTarget : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LidarProgressSetting _settings;
    
    private LidarProgress _progress;
    private LidarStateMachine _fsm;

    public LidarProgressSetting Settings => _settings;
    
    public bool IsProgressComplete => _progress == null ? false : _progress.IsActivated;
    public bool CanInteract => _progress.CanInteract;
    public float CurrentProgress => _progress.CurrentProgress;
    public float RequiredProgress => _progress.RequiredProgress;
    public float ProgressRatio => _progress.ProgressRatio;
    
    public ELidarTargetState State => _fsm.CurrentStateType;
    
    public event Action<float> OnProgressChanged; //ratio전달
    public event Action OnScanComplete;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (IsProgressComplete == true)
        {
            return;
        }
        
        _fsm.Tick(Time.deltaTime);
    }

    public void Init()
    {
        _progress = new LidarProgress(_settings);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += HandleActivated;

        //필수
        _fsm = new LidarStateMachine(this);
        ChangeState(ELidarTargetState.Default, true);
    }
    
    [ContextMenu("리셋")]
    public void ResetAll()
    {
        _progress.Reset();                      //진행도 리셋
        ChangeState(ELidarTargetState.Default); //스테이트 리셋
    }


    #region Connector
    //스캔시 1회 실행
    public void OnScanning(float deltaTime)
    {
        if (IsProgressComplete == true)
        {
            return;
        }

        _fsm.OnScanning(deltaTime);
    }
    
    //스캔 중간 종료시 1회실행
    public void OnScanLost()
    {
        if (IsProgressComplete == true)
        {
            return;
        }

        _fsm.OnScanLost();
    }
    
    #endregion
    
    #region FSM
    public void ChangeState(ELidarTargetState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }
    
    public void ChangeState(ELidarTargetState nextState)
    {
        ChangeState(nextState, false);
    }
    #endregion

    #region Progress
    public void AddProgress(float amount)
    {
        _progress.Add(amount);

        if (_progress.IsActivated == true)
        {
            return;
        }

        // if (_minigame.ShouldEnterMinigame(State) == true)
        // {
        //     ChangeState(ELidarTargetState.OnMinigame);
        // }
    }

    public void ReduceProgress(float amount)
    {
        _progress.Reduce(amount);
    }

    public void ReduceProgressByReturn(float deltaTime)
    {
        _progress.Reduce(_settings.ReturnSpeed * deltaTime);

        if (_progress.CurrentProgress <= 0.0f)
        {
            ChangeState(ELidarTargetState.Default);
        }
    }
    #endregion
    

    private void HandleProgressChanged(float ratio)
    {
        OnProgressChanged?.Invoke(ratio);
    }

    private void HandleActivated()
    {
        OnScanComplete?.Invoke();
    }
}