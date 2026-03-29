using UnityEngine;

[DisallowMultipleComponent]
public class InstanceCondition : MonoBehaviour, IUseCondition
{
    private enum StateValueType
    {
        Bool,
        Int,
        String
    }

    [SerializeField] private RuntimeView runtimeView;
    [SerializeField] private StateKeySO _stateKey;
    [SerializeField] private StateValueType _valueType = StateValueType.Bool;
    [SerializeField] private bool _expectedBoolValue;
    [SerializeField] private int _expectedIntValue;
    [SerializeField] private string _expectedStringValue = string.Empty;

    public bool CanUse(UseContext context)
    {
        if (_stateKey == null)
        {
            return false;
        }

        if (runtimeView == null)
        {
            runtimeView = GetComponentInParent<RuntimeView>();
        }

        RuntimeItemData runtimeItemData = runtimeView != null ? runtimeView.RuntimeItemData : null;
        if (runtimeItemData?.State == null)
        {
            return false;
        }

        return _valueType switch
        {
            StateValueType.Bool => runtimeItemData.State.GetBool(_stateKey) == _expectedBoolValue,
            StateValueType.Int => runtimeItemData.State.GetInt(_stateKey) == _expectedIntValue,
            StateValueType.String => runtimeItemData.State.GetString(_stateKey) == _expectedStringValue,
            _ => false
        };
    }
}
