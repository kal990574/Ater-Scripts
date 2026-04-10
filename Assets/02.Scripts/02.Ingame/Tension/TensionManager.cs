using Sirenix.OdinInspector;
using UnityEngine;

public class TensionManager : MonoBehaviour
{
    [Header("Base Tension / 지속적으로 누적되는 긴장")]
    [SerializeField] private float _baseTension;
    [SerializeField] private float _baseTensionMax = 100.0f;
    [SerializeField] private float _baseTensionAutoIncreasePerSecond = 1.0f;

    [Header("Spike Tension / 순간적으로 변동하고 자연 감소하는 긴장")]
    [SerializeField] private float _spikeTension;
    [SerializeField] private float _spikeTensionMax = 50.0f;
    [SerializeField] private float _spikeTensionDecayPerSecond = 8.0f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog;

    private CompositeSubscription _subscriptions;

    public float BaseTension => _baseTension;
    public float SpikeTension => _spikeTension;
    public float TotalTension => _baseTension + _spikeTension;

    private void OnEnable()
    {
        _subscriptions?.Dispose();
        _subscriptions = new CompositeSubscription();

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            Debug.LogWarning("[TensionManager] GameEventHub.Instance is null.");
            return;
        }

        _subscriptions.Add(hub.Subscribe<OnTensionChangedEvent>(OnTensionChanged));
    }

    private void OnDisable()
    {
        _subscriptions?.Dispose();
        _subscriptions = null;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        IncreaseBaseTensionOverTime(deltaTime);
        DecreaseSpikeTensionOverTime(deltaTime);
    }

    [Button]
    public void ResetTension()
    {
        _baseTension = 0.0f;
        _spikeTension = 0.0f;

        LogState("ResetTension");
    }
    
    private void OnTensionChanged(OnTensionChangedEvent tensionEvent)
    {
        if (string.IsNullOrEmpty(tensionEvent.Reason) == true)
        {
            Debug.LogWarning("[TensionManager] Tension reason is null or empty.");
            return;
        }
        
        if (tensionEvent.Channel == ETensionChannel.BaseTension)
        {
            ApplyBaseTensionDelta(tensionEvent.Amount, tensionEvent.Reason);
        }
        else
        {
            ApplySpikeTensionDelta(tensionEvent.Amount, tensionEvent.Reason);
        }
    }

    [Button]
    public void BaseTensionChange(float delta)
    {
        if (Mathf.Approximately(delta, 0.0f) == true)
        {
            return;
        }

        _baseTension = ClampBaseTension(_baseTension + delta);

        LogState($"ApplyBaseTensionDelta | Reason: User Debug | Delta: {delta:+0.00;-0.00}");
    }
    
    [Button]
    public void SpikeTensionChange(float delta)
    {
        if (Mathf.Approximately(delta, 0.0f) == true)
        {
            return;
        }

        _spikeTension = ClampSpikeTension(_spikeTension + delta);

        LogState($"ApplySpikeTensionDelta | Reason: User Debug | Delta: {delta:+0.00;-0.00}");
    }
    
    private void ApplyBaseTensionDelta(float delta, string reason)
    {
        if (Mathf.Approximately(delta, 0.0f) == true)
        {
            return;
        }

        float previousBase = _baseTension;
        float newBase = _baseTension + delta;

        // 1. Base가 0 아래로 내려가는 경우
        if (newBase < 0.0f)
        {
            float overflow = newBase; // 음수값 그대로

            _baseTension = 0.0f;

            // 남은 값을 Spike에 적용
            ApplySpikeTensionDelta(overflow, $"{reason} (Overflow from Base)");
        }
        else
        {
            _baseTension = ClampBaseTension(newBase);
        }

        LogState($"ApplyBaseTensionDelta | Reason: {reason} | Delta: {delta:+0.00;-0.00}");
    }
    
    private void ApplySpikeTensionDelta(float delta, string reason)
    {
        if (Mathf.Approximately(delta, 0.0f) == true)
        {
            return;
        }

        _spikeTension = ClampSpikeTension(_spikeTension + delta);

        LogState($"ApplySpikeTensionDelta | Reason: {reason} | Delta: {delta:+0.00;-0.00}");
    }

    private void IncreaseBaseTensionOverTime(float deltaTime)
    {
        if (_baseTensionAutoIncreasePerSecond <= 0.0f)
        {
            return;
        }

        float increaseDelta = _baseTensionAutoIncreasePerSecond * deltaTime;
        _baseTension = ClampBaseTension(_baseTension + increaseDelta);
    }

    private void DecreaseSpikeTensionOverTime(float deltaTime)
    {
        if (_spikeTensionDecayPerSecond <= 0.0f)
        {
            return;
        }

        float decreaseDelta = _spikeTensionDecayPerSecond * deltaTime;
        _spikeTension = ClampSpikeTension(_spikeTension - decreaseDelta);
    }

    private float ClampBaseTension(float value)
    {
        return Mathf.Clamp(value, 0.0f, _baseTensionMax);
    }

    private float ClampSpikeTension(float value)
    {
        return Mathf.Clamp(value, 0.0f, _spikeTensionMax);
    }
    
    private void LogState(string action)
    {
        if (_enableDebugLog == false)
        {
            return;
        }

        Debug.Log(
            $"[TensionManager] {action} | " +
            $"Base: {_baseTension:F2}, " +
            $"Spike: {_spikeTension:F2}, " +
            $"Total: {TotalTension:F2}");
    }
}
