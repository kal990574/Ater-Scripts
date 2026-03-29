using UnityEngine;

public class WorldBox : StateApplierBase
{
    [SerializeField] private ScannableObject scannableObject;
    [SerializeField] private Interactable interactable;
    [SerializeField] private GameObject _rewardVisual;
    [SerializeField] private StateKeySO _scanCompleteStateKey;
    [SerializeField] private StateKeySO _openStateKey;
    [SerializeField] private StateKeySO _rewardCollectedStateKey;

    private void Awake()
    {
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

        bool isScanComplete = binder.RuntimeItemData.State.GetBool(_scanCompleteStateKey);
        bool isOpened = binder.RuntimeItemData.State.GetBool(_openStateKey);
        bool rewardCollected = binder.RuntimeItemData.State.GetBool(_rewardCollectedStateKey);

        if (isScanComplete && scannableObject != null)
        {
            scannableObject.ForceScanComplete();
            interactable.SetActivate(true);
        }
        
        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
    
}
