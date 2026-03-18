using UnityEngine;

public class BoxInteraction : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject rewardObject;  // 열쇠 오브젝트 (처음엔 비활성화)

    private bool _isOpened = false;

    public void Interact()
    {
        if (_isOpened) return;

        _isOpened = true;

        // 상자 열기 → 열쇠 오브젝트 활성화
        rewardObject.SetActive(true);

        Debug.Log("상자 열림");
    }
}