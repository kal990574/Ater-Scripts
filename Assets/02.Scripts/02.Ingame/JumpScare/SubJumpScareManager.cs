using System.Collections.Generic;
using UnityEngine;

public class SubJumpScareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private MonoBehaviour playerModeProviderBehaviour;
    [SerializeField] private MonoBehaviour tensionProviderBehaviour;
    [SerializeField] private MonoBehaviour tensionModifierBehaviour;

    [Header("Definitions")]
    [SerializeField] private List<SubJumpScareDefinitionSO> definitions = new List<SubJumpScareDefinitionSO>();

    [Header("Timing")]
    [SerializeField] private float evaluateIntervalSeconds = 1.0f;
    [SerializeField] private bool isMainJumpScareRunning = false;

    private readonly Dictionary<ESubJumpScareType, ISubJumpScareDirector> _directors =
        new Dictionary<ESubJumpScareType, ISubJumpScareDirector>();

    private readonly Dictionary<string, float> _cooldownEndTimes =
        new Dictionary<string, float>();

    private IPlayerModeProvider _playerModeProvider;
    private ITensionProvider _tensionProvider;
    private ITensionModifier _tensionModifier;
    private JumpScareExecutionGate _executionGate;

    private float _nextEvaluateTime;
    private SubJumpScareHandle _currentHandle;

    private void Awake()
    {
        _playerModeProvider = playerModeProviderBehaviour as IPlayerModeProvider;
        _tensionProvider = tensionProviderBehaviour as ITensionProvider;
        _tensionModifier = tensionModifierBehaviour as ITensionModifier;

        _executionGate = new JumpScareExecutionGate();
        _nextEvaluateTime = 0.0f;

        CacheDirectors();
    }

    private void Update()
    {
        UpdateCurrentHandle();

        if (Time.time < _nextEvaluateTime)
        {
            return;
        }

        _nextEvaluateTime = Time.time + evaluateIntervalSeconds;
        TryExecuteRandomSubJumpScare();
    }

    private void CacheDirectors()
    {
        ISubJumpScareDirector[] foundDirectors = GetComponentsInChildren<ISubJumpScareDirector>(true);

        for (int index = 0; index < foundDirectors.Length; index++)
        {
            ISubJumpScareDirector director = foundDirectors[index];

            if (_directors.ContainsKey(director.Type) == true)
            {
                continue;
            }

            _directors.Add(director.Type, director);
        }
    }

    private void UpdateCurrentHandle()
    {
        if (_currentHandle == null)
        {
            return;
        }

        if (_currentHandle.IsCompleted == false)
        {
            return;
        }

        ApplyExecutionResult(_currentHandle.Result);
        _currentHandle = null;
    }

    public bool TryExecuteRandomSubJumpScare()
    {
        if (_currentHandle != null)
        {
            return false;
        }

        if (_playerModeProvider == null ||
            _tensionProvider == null ||
            _tensionModifier == null ||
            playerTransform == null)
        {
            return false;
        }

        JumpScareRuntimeContext context = BuildContext();
        List<SubJumpScareDefinitionSO> candidates = GatherCandidates(context);

        if (candidates.Count <= 0)
        {
            return false;
        }

        SubJumpScareDefinitionSO selectedDefinition = SelectHighestPriorityCandidate(candidates);

        if (selectedDefinition == null)
        {
            return false;
        }

        if (_directors.TryGetValue(selectedDefinition.Type, out ISubJumpScareDirector director) == false)
        {
            return false;
        }

        EJumpScareIntensity intensity = DecideIntensity(context.CurrentTension, selectedDefinition);

        if (director.CanExecute(context, selectedDefinition) == false)
        {
            return false;
        }

        _currentHandle = director.Execute(context, selectedDefinition, intensity);

        if (_currentHandle == null)
        {
            return false;
        }

        RegisterCooldown(selectedDefinition);

        return true;
    }

    private JumpScareRuntimeContext BuildContext()
    {
        float currentTension = _tensionProvider.GetCurrentTension();
        EPlayerInteractMode playerMode = _playerModeProvider.GetCurrentMode();

        return new JumpScareRuntimeContext(
            currentTension,
            playerTransform.position,
            playerTransform.forward,
            playerMode,
            isMainJumpScareRunning,
            _currentHandle != null,
            string.Empty);
    }

    private List<SubJumpScareDefinitionSO> GatherCandidates(JumpScareRuntimeContext context)
    {
        List<SubJumpScareDefinitionSO> results = new List<SubJumpScareDefinitionSO>();

        for (int index = 0; index < definitions.Count; index++)
        {
            SubJumpScareDefinitionSO definition = definitions[index];

            if (definition == null)
            {
                continue;
            }

            if (IsCoolingDown(definition.Id) == true)
            {
                continue;
            }

            if (_executionGate.CanExecute(context, definition) == false)
            {
                continue;
            }

            if (_directors.TryGetValue(definition.Type, out ISubJumpScareDirector director) == false)
            {
                continue;
            }

            if (director.CanExecute(context, definition) == false)
            {
                continue;
            }

            results.Add(definition);
        }

        return results;
    }

    private SubJumpScareDefinitionSO SelectHighestPriorityCandidate(List<SubJumpScareDefinitionSO> candidates)
    {
        SubJumpScareDefinitionSO best = null;

        for (int index = 0; index < candidates.Count; index++)
        {
            SubJumpScareDefinitionSO current = candidates[index];

            if (best == null)
            {
                best = current;
                continue;
            }

            if (current.Priority > best.Priority)
            {
                best = current;
                continue;
            }

            if (current.Priority == best.Priority &&
                current.Weight > best.Weight)
            {
                best = current;
            }
        }

        return best;
    }

    private EJumpScareIntensity DecideIntensity(float tension, SubJumpScareDefinitionSO definition)
    {
        float range = definition.MaxTension - definition.MinTension;

        if (range <= 0.01f)
        {
            return EJumpScareIntensity.Weak;
        }

        float normalized = Mathf.InverseLerp(definition.MinTension, definition.MaxTension, tension);

        if (normalized < 0.33f)
        {
            return EJumpScareIntensity.Weak;
        }

        if (normalized < 0.66f)
        {
            return EJumpScareIntensity.Medium;
        }

        return EJumpScareIntensity.Strong;
    }

    private void ApplyExecutionResult(SubJumpScareExecutionResult result)
    {
        if (result.TensionDelta > 0.0f)
        {
            _tensionModifier.AddTension(result.TensionDelta, result.Reason);
            return;
        }

        if (result.TensionDelta < 0.0f)
        {
            _tensionModifier.DecreaseTension(-result.TensionDelta, result.Reason);
        }
    }

    private void RegisterCooldown(SubJumpScareDefinitionSO definition)
    {
        if (string.IsNullOrEmpty(definition.Id) == true)
        {
            return;
        }

        _cooldownEndTimes[definition.Id] = Time.time + definition.CooldownSeconds;
    }

    private bool IsCoolingDown(string definitionId)
    {
        if (string.IsNullOrEmpty(definitionId) == true)
        {
            return false;
        }

        if (_cooldownEndTimes.TryGetValue(definitionId, out float endTime) == false)
        {
            return false;
        }

        return Time.time < endTime;
    }

    public void SetMainJumpScareRunning(bool isRunning)
    {
        isMainJumpScareRunning = isRunning;
    }
}