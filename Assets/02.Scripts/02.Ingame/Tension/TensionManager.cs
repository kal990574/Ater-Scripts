using UnityEngine;

public class TensionManager : MonoBehaviour
{
    [Header("Rule Table")]
    [SerializeField] private TensionRuleTableSO _ruleTable;

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

    private readonly CompositeSubscription _subscriptions = new CompositeSubscription();

    public float BaseTension => _baseTension;
    public float SpikeTension => _spikeTension;
    public float TotalTension => _baseTension + _spikeTension;

    private void OnEnable()
    {
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
        _subscriptions.Dispose();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        IncreaseBaseTensionOverTime(deltaTime);
        DecreaseSpikeTensionOverTime(deltaTime);
    }

    public void ResetTension()
    {
        _baseTension = 0.0f;
        _spikeTension = 0.0f;

        LogState("ResetTension");
    }

    public void SetRuleTable(TensionRuleTableSO ruleTable)
    {
        _ruleTable = ruleTable;

        if (_ruleTable == null)
        {
            Debug.LogWarning("[TensionManager] RuleTable is null after SetRuleTable.");
            return;
        }

        LogState("SetRuleTable");
    }

    private void OnTensionChanged(OnTensionChangedEvent tensionEvent)
    {
        if (_ruleTable == null)
        {
            Debug.LogWarning("[TensionManager] RuleTable is null.");
            return;
        }

        if (string.IsNullOrEmpty(tensionEvent.Reason) == true)
        {
            Debug.LogWarning("[TensionManager] Tension reason is null or empty.");
            return;
        }

        if (_ruleTable.TryGetRule(tensionEvent.Reason, out TensionRule rule) == false)
        {
            WarnUnknownReason(tensionEvent.Reason);
            return;
        }

        ApplyRule(rule);
    }

    private void ApplyRule(TensionRule rule)
    {
        if (rule == null)
        {
            Debug.LogWarning("[TensionManager] Rule is null.");
            return;
        }

        switch (rule.Channel)
        {
            case ETensionChannel.BaseTension:
            {
                ApplyBaseTensionDelta(rule.Delta, rule.Reason);
                break;
            }
            case ETensionChannel.SpikeTension:
            {
                ApplySpikeTensionDelta(rule.Delta, rule.Reason);
                break;
            }
            default:
            {
                Debug.LogWarning($"[TensionManager] Unsupported Channel : {rule.Channel}");
                break;
            }
        }
    }

    private void ApplyBaseTensionDelta(float delta, string reason)
    {
        if (Mathf.Approximately(delta, 0.0f) == true)
        {
            return;
        }

        _baseTension = ClampBaseTension(_baseTension + delta);

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

    private void WarnUnknownReason(string reason)
    {
        Debug.LogWarning($"[TensionManager] Unknown Reason : {reason}");
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