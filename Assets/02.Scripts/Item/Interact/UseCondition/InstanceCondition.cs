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

    public bool CanUse(UseContext context)
    {
        if (_stateKey == null)
        {
            return false;
        }

        if (_instanceView == null)
        {
            _instanceView = GetComponentInParent<InstanceView>();
        }

        ItemInstanceData itemInstanceData = _instanceView != null ? _instanceView.ItemInstanceData : null;
        if (itemInstanceData?.State == null)
        {
            return false;
        }

        return _valueType switch
        {
            StateValueType.Bool => itemInstanceData.State.GetBool(_stateKey) == _expectedBoolValue,
            StateValueType.Int => itemInstanceData.State.GetInt(_stateKey) == _expectedIntValue,
            StateValueType.String => itemInstanceData.State.GetString(_stateKey) == _expectedStringValue,
            _ => false
        };
    }
}
