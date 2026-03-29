using UnityEngine;

//기본적으로 스캔 동기화
public class ScanStateApplier : BindApplierBase
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
        if (binder?.ItemInstanceData == null)
        {
            return;
        }

        bool isScanComplete = binder.ItemInstanceData.State.GetBool(_scanCompleteStateKey);
      
        if (isScanComplete && _scannableObject != null)
        {
            _scannableObject.ForceScanComplete();
        }
    }
}
