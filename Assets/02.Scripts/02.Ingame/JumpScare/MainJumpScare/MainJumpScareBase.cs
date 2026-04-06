using UnityEngine;

public abstract class MainJumpScareBase : MonoBehaviour
{
    [Header("Main Jump Scare")]
    [SerializeField] private MainJumpScareKeyReference _id;
    [SerializeField] private bool _canActive;
    [SerializeField] private EMainJumpScareState _state = EMainJumpScareState.Waiting;

    public string Id => _id;
    public bool CanActive => _canActive;
    public EMainJumpScareState State => _state;

    protected virtual void Awake()
    {
        if (_state == EMainJumpScareState.None)
        {
            _state = EMainJumpScareState.Waiting;
        }
    }

    public void SetCanActive(bool canActive)
    {
        if (_state == EMainJumpScareState.Finished)
        {
            return;
        }

        _canActive = canActive;
    }

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(_id) == true)
        {
            Debug.LogError($"{name} 메인 점프스케어의 ID가 비어 있어 실행할 수 없습니다.", this);
            return;
        }

        if (_canActive == false)
        {
            Debug.LogWarning($"메인 점프스케어 [{_id}] 는 아직 활성 조건이 열리지 않아 실행할 수 없습니다.", this);
            return;
        }

        if (_state == EMainJumpScareState.Playing)
        {
            Debug.LogWarning($"메인 점프스케어 [{_id}] 는 이미 실행 중입니다.", this);
            return;
        }

        if (_state == EMainJumpScareState.Finished)
        {
            Debug.LogWarning($"메인 점프스케어 [{_id}] 는 이미 종료되어 다시 실행할 수 없습니다.", this);
            return;
        }

        _state = EMainJumpScareState.Playing;
        OnExecute();
    }

    protected abstract void OnExecute();

    protected void NotifyFinished()
    {
        if (_state != EMainJumpScareState.Playing)
        {
            Debug.LogWarning($"메인 점프스케어 [{_id}] 가 Playing 상태가 아닌데 종료 통지를 시도했습니다.", this);
        }

        _state = EMainJumpScareState.Finished;
        _canActive = false;

        if (JumpScareManager.Instance == null)
        {
            Debug.LogError($"JumpScareManager.Instance 가 없어 메인 점프스케어 [{_id}] 종료를 통지할 수 없습니다.", this);
            return;
        }

        JumpScareManager.Instance.NotifyMainJumpScareFinished(_id);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_state == EMainJumpScareState.None)
        {
            _state = EMainJumpScareState.Waiting;
        }
    }
#endif
}
