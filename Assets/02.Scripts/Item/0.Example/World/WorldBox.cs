using UnityEngine;

public class WorldBox : BindApplierBase
{
    [SerializeField] private ScannableObject _scannableObject;
    [SerializeField] private InteractableObject _interactableObject;
    [SerializeField] private GameObject _rewardVisual;

    private void Awake()
    {
        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInChildren<ScannableObject>();
        }
    }

    public override void ApplyState(ItemBinderBase binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        bool isScanComplete = binder.ItemInstance.State.GetBool(BinderContext.IS_SCAN_COMPLETE);
        bool isOpened = binder.ItemInstance.State.GetBool(BinderContext.IS_OPEN);
        bool rewardCollected = binder.ItemInstance.State.GetBool(BinderContext.IS_REWARD_COLLECTED);

        if (isScanComplete && _scannableObject != null)
        {
            _scannableObject.ForceScanComplete();
        }

        if (isScanComplete && _scannableObject != null)
        {
            _interactableObject.SetActivate();
        }

        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
    
}
