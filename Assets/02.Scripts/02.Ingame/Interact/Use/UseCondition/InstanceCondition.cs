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
    [SerializeField] private string _stateKey = string.Empty;
    [SerializeField] private StateValueType _valueType = StateValueType.Bool;
    [SerializeField] private bool _expectedBoolValue;
    [SerializeField] private int _expectedIntValue;
    [SerializeField] private string _expectedStringValue = string.Empty;

    public bool CanUse(InteractionContext context)
    {
        if (string.IsNullOrWhiteSpace(_stateKey))
        {
            return false;
        }

        if (runtimeView == null)
        {
            runtimeView = GetComponentInParent<RuntimeView>();
        }

        RuntimeData runtimeData = runtimeView != null ? runtimeView.RuntimeData : null;
        if (runtimeData?.State == null)
        {
            return false;
        }

        return _valueType switch
        {
            StateValueType.Bool => runtimeData.State.GetBool(_stateKey) == _expectedBoolValue,
            StateValueType.Int => runtimeData.State.GetInt(_stateKey) == _expectedIntValue,
            StateValueType.String => runtimeData.State.GetString(_stateKey) == _expectedStringValue,
            _ => false
        };
    }
}
