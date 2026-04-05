using UnityEngine;

public class WorldBox : StateApplierBase
{
    [SerializeField] private ScannableObject scannableObject;
    [SerializeField] private Interactable interactable;
    [SerializeField] private GameObject _rewardVisual;
    [SerializeField] private string _openStateKey = string.Empty;
    [SerializeField] private string _rewardCollectedStateKey = string.Empty;

    protected override void OnAwake()
    {
        base.OnAwake();

        if (scannableObject == null)
        {
            scannableObject = GetComponentInChildren<ScannableObject>();
        }
    }

    public override void ApplyState(RuntimeView binder)
    {
        if (binder?.RuntimeItemData == null)
        {
            return;
        }
        
        bool isOpened = binder.RuntimeItemData.State.GetBool(_openStateKey);
        bool rewardCollected = binder.RuntimeItemData.State.GetBool(_rewardCollectedStateKey);
        
        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
    
}
