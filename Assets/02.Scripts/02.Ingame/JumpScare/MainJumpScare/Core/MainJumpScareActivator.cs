using UnityEngine;

public class MainJumpScareActivator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _mainJumpScareId;

    [Header("Default")]
    [SerializeField] private bool _activeValue = true;

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
        if (SubJumpScareManager.Instance == null)
        {
            Debug.LogError("JumpScareManager.Instance 가 없어 메인 점프스케어 CanActive 를 변경할 수 없습니다.", this);
            return;
        }

        SubJumpScareManager.Instance.SetMainJumpScareCanActive(_mainJumpScareId, activeValue);
    }
}