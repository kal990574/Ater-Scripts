using UnityEngine;

public class TensionManager : MonoBehaviour
{
    [Header("Slow Tension/느리게 증가. 이벤트를 통해서만 감소")]
    [SerializeField] private float _slowTension;
    [SerializeField] private float _slowTensionMax = 100.0f;
    [SerializeField] private float _slowTensionAutoIncreasePerSecond = 1.0f;

    [Header("Fast Tension/빠르게 감소. 이벤트를 통해서만 증가")]
    [SerializeField] private float _fastTension;
    [SerializeField] private float _fastTensionMax = 50.0f;
    [SerializeField] private float _fastTensionDecayPerSecond = 8.0f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog;

    public float SlowTension => _slowTension;
    public float FastTension => _fastTension;
    public float FinalTension => _slowTension + _fastTension;

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        IncreaseSlowTensionOverTime(deltaTime);
        DecreaseFastTensionOverTime(deltaTime);
    }

    public void ResetTension()
    {
        _slowTension = 0.0f;
        _fastTension = 0.0f;

        LogState("ResetTension");
    }

    public void AddSlowTension(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _slowTension = ClampSlowTension(_slowTension + amount);

        LogState($"AddSlowTension : +{amount:F2}");
    }

    public void ReduceSlowTension(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _slowTension = ClampSlowTension(_slowTension - amount);

        LogState($"ReduceSlowTension : -{amount:F2}");
    }

    public void AddFastTension(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _fastTension = ClampFastTension(_fastTension + amount);

        LogState($"AddFastTension : +{amount:F2}");
    }

    public void ReduceFastTension(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _fastTension = ClampFastTension(_fastTension - amount);

        LogState($"ReduceFastTension : -{amount:F2}");
    }

    public void ReduceFinalTension(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        float remainingAmount = amount;

        remainingAmount = ReduceFastTensionFirst(remainingAmount);
        remainingAmount = ReduceSlowTensionNext(remainingAmount);

        LogState($"ReduceFinalTension : -{amount:F2}");
    }

    private void IncreaseSlowTensionOverTime(float deltaTime)
    {
        if (_slowTensionAutoIncreasePerSecond <= 0.0f)
        {
            return;
        }

        float increaseAmount = _slowTensionAutoIncreasePerSecond * deltaTime;
        _slowTension = ClampSlowTension(_slowTension + increaseAmount);
    }

    private void DecreaseFastTensionOverTime(float deltaTime)
    {
        if (_fastTensionDecayPerSecond <= 0.0f)
        {
            return;
        }

        float decreaseAmount = _fastTensionDecayPerSecond * deltaTime;
        _fastTension = ClampFastTension(_fastTension - decreaseAmount);
    }

    private float ReduceFastTensionFirst(float remainingAmount)
    {
        if (remainingAmount <= 0.0f)
        {
            return 0.0f;
        }

        if (_fastTension <= 0.0f)
        {
            return remainingAmount;
        }

        float reduceAmount = Mathf.Min(_fastTension, remainingAmount);
        _fastTension = ClampFastTension(_fastTension - reduceAmount);

        return remainingAmount - reduceAmount;
    }

    private float ReduceSlowTensionNext(float remainingAmount)
    {
        if (remainingAmount <= 0.0f)
        {
            return 0.0f;
        }

        if (_slowTension <= 0.0f)
        {
            return remainingAmount;
        }

        float reduceAmount = Mathf.Min(_slowTension, remainingAmount);
        _slowTension = ClampSlowTension(_slowTension - reduceAmount);

        return remainingAmount - reduceAmount;
    }

    private float ClampSlowTension(float value)
    {
        return Mathf.Clamp(value, 0.0f, _slowTensionMax);
    }

    private float ClampFastTension(float value)
    {
        return Mathf.Clamp(value, 0.0f, _fastTensionMax);
    }

    private void LogState(string action)
    {
        if (_enableDebugLog == false)
        {
            return;
        }

        Debug.Log(
            $"[TensionManager] {action} | Slow: {_slowTension:F2}, Fast: {_fastTension:F2}, Final: {FinalTension:F2}");
    }
}