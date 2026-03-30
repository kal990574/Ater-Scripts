using UnityEngine;

public class TimingQTETester : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private UI_CircleTimingQTE _timingView;
    [SerializeField] private TimingQuickTimeEventConfig _config;

    [Header("Input")]
    [SerializeField] private KeyCode _spawnKey = KeyCode.F;

    [Header("Auto Trigger")]
    [SerializeField] private bool _useAutoTrigger = true;
    [Min(0.1f)]
    [SerializeField] private float _interval = 5f;

    private TimingQTERunner _skillCheck;
    private float _timer;

    private void Awake()
    {
        if (_timingView == null || _config == null)
        {
            Debug.LogError($"[{nameof(TimingQTETester)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        _skillCheck = new TimingQTERunner(_config, _timingView);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_spawnKey))
        {
            TryStartSkillCheck();
        }

        if (_useAutoTrigger)
        {
            _timer += Time.deltaTime;
            if (_timer >= _interval)
            {
                _timer = 0f;
                TryStartSkillCheck();
            }
        }
    }

    private void TryStartSkillCheck()
    {
        if (_skillCheck == null || _skillCheck.IsPlaying)
        {
            return;
        }

        _skillCheck.Begin();
    }
}
