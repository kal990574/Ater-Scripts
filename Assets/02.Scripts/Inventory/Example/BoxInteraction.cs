using UnityEngine;

public class BoxInteraction : MonoBehaviour, IInteractableUI
{
    [SerializeField] private GameObject _rewardObject;

    private bool _isOpened = false;

    public void Interact()
    {
        if (_isOpened) return;

        _isOpened = true;

        _rewardObject.SetActive(true);

        Debug.Log("상자 열림");
    }
}