using UnityEngine;

//어차피 스캔을 못하면 인터렉트가 불가능하지만 혹시나 조건체크
public class ScanCondition : MonoBehaviour,IUseCondition
{
    [SerializeField] private bool _completeToTrue = false;
    public bool CanUse(UseContext context)
    {
        if (context?.Target != null &&
            context.Target.TryGetComponent(out IScannable scannableObject) &&
            scannableObject.IsProgressComplete == _completeToTrue)
        {
            return true;
        }
        return false;
    }
}
