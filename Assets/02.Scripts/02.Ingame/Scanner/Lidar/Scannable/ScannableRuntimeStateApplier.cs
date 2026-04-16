using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class ScannableRuntimeStateApplier : StateApplierBase
{
    private const string ScanStateKey = "is_scan";

    [TabGroup("Inspector", "References")]
    [Required]
    [LabelText("Scannable Object")]
    [SerializeField] private ScannableObject _scannableObject;

    protected override void OnAwake()
    {
        if (_scannableObject == null)
        {
            _scannableObject = GetComponent<ScannableObject>();
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInParent<ScannableObject>();
        }
    }

    public override void ApplyState(RuntimeView binder)
    {
        if (_runtimeView != null && binder != null && !ReferenceEquals(_runtimeView, binder))
        {
            return;
        }

        if (_runtimeView == null)
        {
            _runtimeView = binder;
        }

        _isBind = _runtimeView != null;

        if (_scannableObject == null || !CheckBindValid())
        {
            return;
        }

        bool isScanCompleted = _runtimeView.RuntimeData.State.GetBool(ScanStateKey);
        _scannableObject.ApplyRuntimeScanState(isScanCompleted);
    }
}
