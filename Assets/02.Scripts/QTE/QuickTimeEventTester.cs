using UnityEngine;
using UnityEngine.Serialization;

public class QuickTimeEventTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CircleTimingQTEUI _timingView;

    [Header("Input")]
    [SerializeField] private KeyCode _submitKey = KeyCode.Space;
    [SerializeField] private KeyCode _startKey = KeyCode.F;
    [SerializeField] private KeyCode _cancelKey = KeyCode.Escape;

    [FormerlySerializedAs("_settings")]
    [Header("Timing QTE Settings")]
    [SerializeField] private TimingQuickTimeEventConfig config;

    private IQuickTimeEvent _currentEvent;
    private TimingQTERunner _timingEvent;

    private void Awake()
    {
        _timingEvent = new TimingQTERunner(config, _timingView);
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
        if (Input.GetKeyDown(_startKey) == true)
        {
            StartTimingQte();
        }

        if (_currentEvent == null)
        {
            return;
        }

        if (_currentEvent.IsPlaying == true)
        {
            _currentEvent.Tick(Time.deltaTime);

            if (Input.GetKeyDown(_submitKey) == true)
            {
                _currentEvent.Submit();
            }

            if (Input.GetKeyDown(_cancelKey) == true)
            {
                _currentEvent.Cancel();
            }
        }
    }

    public void StartTimingQte()
    {
        if (_currentEvent != null && _currentEvent.IsPlaying == true)
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