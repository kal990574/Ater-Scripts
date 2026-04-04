using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class RuntimeStateApplier : StateApplierBase
{
    private enum StateValueType
    {
        Bool,
        Int,
        String
    }
    
    [SerializeField] private string _stateKey = string.Empty;
    [SerializeField] private StateValueType _valueType = StateValueType.Bool;
    [SerializeField] private bool _expectedBoolValue;
    [SerializeField] private int _expectedIntValue;
    [SerializeField] private string _expectedStringValue = string.Empty;
    [SerializeField] private bool _invokeOnlyOnChange = true;

    [Header("Events")]
    [SerializeField] private UnityEvent _onMatched;
    [SerializeField] private UnityEvent _onUnmatched;

    private bool? _lastResult;

    
    public override void ApplyState(RuntimeView binder)
    {
        _runtimeView = binder;
        _isBind = binder != null;

        if (!CheckBindValid() || string.IsNullOrWhiteSpace(_stateKey))
        {
            return;
        }

        bool isMatched = EvaluateState(_runtimeView.RuntimeData);
        if (_invokeOnlyOnChange && _lastResult.HasValue && _lastResult.Value == isMatched)
        {
            return;
        }

        _lastResult = isMatched;

        if (isMatched)
        {
            _onMatched?.Invoke();
            return;
        }

        _onUnmatched?.Invoke();
    }

    [ContextMenu("Evaluate")]
    public void Evaluate()
    {
        ApplyState(_runtimeView != null ? _runtimeView : GetComponentInParent<RuntimeView>());
    }

    private bool EvaluateState(RuntimeData runtimeData)
    {
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
