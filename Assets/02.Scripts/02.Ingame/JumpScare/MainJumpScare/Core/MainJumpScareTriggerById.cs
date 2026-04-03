using UnityEngine;

public class MainJumpScareTriggerById : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _mainJumpScareId;

    public void Trigger()
    {
        if (SubJumpScareManager.Instance == null)
        {
            Debug.LogError("JumpScareManager.Instance 가 없어 메인 점프스케어를 실행할 수 없습니다.", this);
            return;
        }

        SubJumpScareManager.Instance.ExecuteMainJumpScare(_mainJumpScareId);
    }
}