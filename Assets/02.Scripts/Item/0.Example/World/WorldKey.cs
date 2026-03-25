using UnityEngine;

public class WorldKey : WorldItemBase
{
    [SerializeField] private ScannableObject _scannableObject;

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
      
        if (isScanComplete && _scannableObject != null)
        {
            _scannableObject.ForceScanComplete();
        }
    }
}
