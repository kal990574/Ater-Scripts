using UnityEngine;

public class WorldBox : BindApplierBase
{
    [SerializeField] private ScannableObject _scannableObject;
    [SerializeField] private InteractableObject _interactableObject;
    [SerializeField] private GameObject _rewardVisual;
    [SerializeField] private StateKeySO _scanCompleteStateKey;
    [SerializeField] private StateKeySO _openStateKey;
    [SerializeField] private StateKeySO _rewardCollectedStateKey;

    private void Awake()
    {
        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInChildren<ScannableObject>();
        }
    }

    public override void ApplyState(InstanceView binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        bool isScanComplete = binder.ItemInstance.State.GetBool(_scanCompleteStateKey);
        bool isOpened = binder.ItemInstance.State.GetBool(_openStateKey);
        bool rewardCollected = binder.ItemInstance.State.GetBool(_rewardCollectedStateKey);

        if (isScanComplete && _scannableObject != null)
        {
            _scannableObject.ForceScanComplete();
            _interactableObject.SetActivate(true);
        }
        
        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
    
}
