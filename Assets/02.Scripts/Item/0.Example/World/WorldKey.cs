using UnityEngine;

public class WorldKey : BindApplierBase
{
    [SerializeField] private ScannableObject _scannableObject;
    [SerializeField] private StateKeySO _scanCompleteStateKey;

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
      
        if (isScanComplete && _scannableObject != null)
        {
            _scannableObject.ForceScanComplete();
        }
    }
}
