using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LidarTarget : MonoBehaviour
{
    [FormerlySerializedAs("shader")]
    [FormerlySerializedAs("_shaderModifier")]
    [Header("Reference")] 
    [SerializeField] private InteractTargetShaderModifier shaderModifier;
    
    [Header("Settings")]
    [SerializeField] private LidarProgressSetting _settings;

    [Header("Minigame 추후 추가 예정")]
    [SerializeField] private MonoBehaviour _minigameProvider;
    
    private LidarProgress _progress;
    private IScanMinigame _currentMinigame;
    private LidarMinigame _minigame;
    private LidarStateMachine _fsm;

    public LidarProgressSetting Settings => _settings;
    
    public bool IsProgressComplete => _progress.IsActivated;
    public bool CanInteract => _progress.CanInteract;
    public float CurrentProgress => _progress.CurrentProgress;
    public float RequiredProgress => _progress.RequiredProgress;
    public float ProgressRatio => _progress.ProgressRatio;
    
    public ELidarTargetState State => _fsm.CurrentStateType;
    public IScanMinigame CurrentMinigame => _currentMinigame;
    
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
        
        //선택
        if (_minigameProvider != null)
        {
            _currentMinigame = _minigameProvider as IScanMinigame;
            _minigame = new LidarMinigame(_settings, _currentMinigame, _progress, ChangeState);
        }
        
        //필수
        _fsm = new LidarStateMachine(this);
        ChangeState(ELidarTargetState.Default, true);
    }
    
    [ContextMenu("리셋")]
    public void ResetAll()
    {
        _minigame.CancelIfPlaying();            //미니게임 리셋
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
    
    #region Minigame
    //미니게임 시작
    public void BeginMinigame()
    {
        _minigame.BeginOrReturnToProgress();
    }

    //미니게임 실행중
    public void TickMinigame(float deltaTime)
    {
        MinigameResult? result = _minigame.Tick(deltaTime);

        if (result.HasValue == true)
        {
            ApplyMinigameResult(result.Value);
        }
    }
    
    //미니게임 판정 : 추후 E키를 눌렀을때
    public void SubmitMinigame()
    {
        _minigame.Submit(State);
    }

    
    //미니게임 종료
    public void EndMinigame()
    {
        _minigame.CancelIfPlaying();
    }
    
    
    //미니게임 종료시 결과 반영
    public void ApplyMinigameResult(MinigameResult result)
    {
        switch (result)
        {
            case MinigameResult.Default:
            {
                ChangeState(ELidarTargetState.OnProgress);
                break;
            }
            case MinigameResult.Fail:
            {
                _progress.Reduce(_settings.FailPenalty);

                if (_progress.CurrentProgress > 0.0f)
                {
                    ChangeState(ELidarTargetState.OnReturn);
                }
                else
                {
                    ChangeState(ELidarTargetState.Default);
                }

                break;
            }
            case MinigameResult.Success:
            {
                ChangeState(ELidarTargetState.OnProgress);
                break;
            }
            case MinigameResult.GreatSuccess:
            {
                _progress.Add(_settings.GreatSuccessBonus);

                if (_progress.IsActivated == false)
                {
                    ChangeState(ELidarTargetState.OnProgress);
                }

                break;
            }
        }
    }
    
    
    //미니게임 도중 스캔이 중단될 경우
    public void ForceFailCurrentMinigame()
    {
        if (State != ELidarTargetState.OnMinigame)
        {
            return;
        }

        _minigame.ForceFail();
        ApplyMinigameResult(MinigameResult.Fail);
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