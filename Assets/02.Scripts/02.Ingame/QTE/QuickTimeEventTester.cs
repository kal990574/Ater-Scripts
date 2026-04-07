using UnityEngine;
using UnityEngine.Serialization;

public class QuickTimeEventTester : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private UI_CircleTimingQTE _timingView;
    [SerializeField] private TimingQuickTimeEventConfig _config;

    [Header("Input")]
    [SerializeField] private KeyCode _submitKey = KeyCode.Space;
    [SerializeField] private KeyCode _startKey = KeyCode.F;
    [SerializeField] private KeyCode _cancelKey = KeyCode.Escape;

    private IQuickTimeEvent _currentEvent;
    private TimingQTERunner _timingEvent;

    private void Start()
    {
        if (_timingView == null || _config == null)
        {
            Debug.LogError($"[{nameof(QuickTimeEventTester)}] Required references are missing.", this);
            enabled = false;
            return;
        }

        _timingEvent = new TimingQTERunner(_qteManager, _config, _timingView);
        _timingEvent.OnEnded += HandleEnded;
    }

    private void OnDestroy()
    {
        if (_timingEvent != null)
        {
            _timingEvent.OnEnded -= HandleEnded;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_startKey))
        {
            StartTimingQte();
        }

        if (_currentEvent == null)
        {
            return;
        }

        if (_currentEvent.IsPlaying)
        {
            _currentEvent.Tick(Time.deltaTime);

            if (Input.GetKeyDown(_submitKey))
            {
                _currentEvent.Submit();
            }

            if (Input.GetKeyDown(_cancelKey))
            {
                _currentEvent.Cancel();
            }
        }
    }

    public void StartTimingQte()
    {
        if (_currentEvent != null && _currentEvent.IsPlaying)
        {
            return;
        }

        _currentEvent = _timingEvent;
        _currentEvent.Begin();
    }

    private void HandleEnded(EQuickTimeEventResult result)
    {
        Debug.Log($"QTE Ended : {result}");
    }
}
