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

    [SerializeField] private InstanceView _instanceView;
    [SerializeField] private StateKeySO _stateKey;
    [SerializeField] private StateValueType _valueType = StateValueType.Bool;
    [SerializeField] private bool _expectedBoolValue;
    [SerializeField] private int _expectedIntValue;
    [SerializeField] private string _expectedStringValue = string.Empty;

    public bool CanUse(UseableObject useableObject)
    {
        if (_stateKey == null)
        {
            return false;
        }

        if (_instanceView == null)
        {
            _instanceView = GetComponentInParent<InstanceView>();
        }

        ItemInstance itemInstance = _instanceView != null ? _instanceView.ItemInstance : null;
        if (itemInstance?.State == null)
        {
            return false;
        }

        return _valueType switch
        {
            StateValueType.Bool => itemInstance.State.GetBool(_stateKey) == _expectedBoolValue,
            StateValueType.Int => itemInstance.State.GetInt(_stateKey) == _expectedIntValue,
            StateValueType.String => itemInstance.State.GetString(_stateKey) == _expectedStringValue,
            _ => false
        };
    }
}
