using UnityEngine;

public class PlayerDetectAbility : PlayerAbility
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);

    private PlayerTargetDetector _playerTargetDetector;
    private IDetectable _currentTarget;
    private GameEventPublisher _eventPublisher;

    private IDetectable _currentPromptTarget;
    private string _lastPublishedDescription;
    private bool _suppressPromptUntilLookAway;

    public IDetectable CurrentTarget => _currentTarget;


    private void Start()
    {
        _camera = Camera.main;
        _playerTargetDetector = new PlayerTargetDetector(
            new RaycastService(),
            _query);

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);
    }

    private void Update()
    {
        IDetectable nextDetectTarget =
            _playerTargetDetector.Detect(_camera.transform.position, _camera.transform.forward);

        if (!ReferenceEquals(_currentTarget, nextDetectTarget))
        {
            _currentTarget?.OnDetectExit();
            _currentTarget = nextDetectTarget;
            _currentTarget?.OnDetectEnter();
        }

        UpdatePromptEvent();
    }

    private void UpdatePromptEvent()
    {
        IDetectable promptTarget = null;

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward,
            out RaycastHit hit, _query.Distance))
        {
            promptTarget = hit.collider.GetComponentInParent<IDetectable>();
        }

        if (_suppressPromptUntilLookAway)
        {
            return;
        }

        string desc = promptTarget?.HoverDescription ?? string.Empty;
        bool changed = !ReferenceEquals(_currentPromptTarget, promptTarget)
                       || desc != _lastPublishedDescription;

        if (!changed) return;

        _currentPromptTarget = promptTarget;
        _lastPublishedDescription = desc;

        bool isVisible = promptTarget != null && !string.IsNullOrEmpty(desc);
        _eventPublisher.TryPublish(ctx => new InteractPromptRawEvent(ctx, isVisible, desc));
    }

    public void ResumePrompt()
    {
        _suppressPromptUntilLookAway = false;
    }

    public void ForceHidePrompt()
    {
        _suppressPromptUntilLookAway = true;
        _lastPublishedDescription = string.Empty;
        _eventPublisher.TryPublish(ctx => new InteractPromptRawEvent(ctx, false, string.Empty));
    }

    private void OnDisable()
    {
        ClearCurrentHoverTarget();
    }

    private void ClearCurrentHoverTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.OnDetectExit();
        _currentTarget = null;

        _currentPromptTarget = null;
        _lastPublishedDescription = string.Empty;
        _eventPublisher.TryPublish(ctx => new InteractPromptRawEvent(ctx, false, string.Empty));
    }
}
