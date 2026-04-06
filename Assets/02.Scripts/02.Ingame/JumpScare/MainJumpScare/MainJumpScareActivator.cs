using UnityEngine;

public class MainJumpScareActivator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected MainJumpScareKeyReference _mainJumpScareId;

    [Header("Default")]
    [SerializeField] private bool _activeValue = true;

    [Header("Debug")]
    [SerializeField] protected bool _enableLog = true;

    public void Apply()
    {
        SetActive(_activeValue);
    }

    public void SetActiveTrue()
    {
        SetActive(true);
    }

    public void SetActiveFalse()
    {
        SetActive(false);
    }

    public void SetActive(bool activeValue)
    {
        if (JumpScareManager.Instance == null)
        {
            Debug.LogError("JumpScareManager.Instance 가 없어 메인 점프스케어 CanActive 를 변경할 수 없습니다.", this);
            return;
        }

        JumpScareManager.Instance.SetMainJumpScareCanActive(_mainJumpScareId.Value, activeValue);

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 메인 점프스케어 [{_mainJumpScareId}] CanActive 를 {activeValue} 로 변경 요청했습니다.", this);
        }
    }

    public void Activate()
    {
        TryActivate();
    }
    
    protected virtual bool TryActivate()
    {
        if (JumpScareManager.Instance == null)
        {
            Debug.LogError("JumpScareManager.Instance 가 없어 메인 점프스케어를 실행할 수 없습니다.", this);
            return false;
        }
        
        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 메인 점프스케어 [{_mainJumpScareId}] 실행 요청.", this);
        }
        
        if(JumpScareManager.Instance.TryExecuteMainJumpScare(_mainJumpScareId.Value) == false)
        {
            Debug.Log($"[{name}] 메인 점프스케어 [{_mainJumpScareId}] 실패.", this);
            return false;
        }

        return true;
    }
}