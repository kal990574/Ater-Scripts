using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    private static JumpScareManager _instance;
    public static JumpScareManager Instance => _instance;

    [SerializeField] private bool _activeSwitch = true;

    
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TensionManager _tensionManager;
    [SerializeField] private Camera _mainCamera;
    [Title("Sub JumpScare")]
    [Header("Executors")]
    [SerializeField] private FakeEnemyJumpScareExecutor _fakeEnemyJumpScareExecutor;
    [SerializeField] private PostProcessSubJumpScareExecutor _postProcessJumpScareExecutor;
    [SerializeField] private SoundSubJumpScareExecutor _soundSubJumpScareExecutor;
    
    [Header("Data")]
    [SerializeField] private SubJumpScareDatabaseSO _database;

    [Header("Periodic Trigger")]
    [SerializeField] private bool _usePeriodicTick = true;
    [SerializeField] private float _periodicInterval = 1f;

    [Header("Sub Debug")]
    [SerializeField] private bool _enableSubLog = true;

    [Header("Main Debug")]
    [SerializeField] private bool _enableMainLog = true;
    [SerializeField] private bool _includeInactiveOnRegister = true;

    private CompositeSubscription _subscriptions;
    private MainJumpScareService _mainJumpScareService;
    private SubJumpScareService _subJumpScareService;

    public bool EnableSubLog => _enableSubLog;
    public bool EnableMainLog => _enableMainLog;
    public bool IncludeInactiveOnRegister => _includeInactiveOnRegister;
    public bool IsActiveSwitchOn => _activeSwitch;
    public Transform PlayerRootTransform => _playerController.transform;
    public Transform PlayerCameraTransform => _mainCamera.transform;

    [ShowInInspector, ReadOnly, FoldoutGroup("Cooldown State")]
    private float GlobalCooldownRemaining => _subJumpScareService?.DebugGlobalCooldownRemaining ?? 0f;

    [ShowInInspector, ReadOnly, FoldoutGroup("Cooldown State")]
    private float SoundTypeCooldownRemaining => _subJumpScareService?.DebugSoundCooldownRemaining ?? 0f;

    [ShowInInspector, ReadOnly, FoldoutGroup("Cooldown State")]
    private float PostProcessTypeCooldownRemaining => _subJumpScareService?.DebugPostProcessCooldownRemaining ?? 0f;

    [ShowInInspector, ReadOnly, FoldoutGroup("Cooldown State")]
    private float FakeEnemyTypeCooldownRemaining => _subJumpScareService?.DebugFakeEnemyCooldownRemaining ?? 0f;

    [ShowInInspector, ReadOnly, FoldoutGroup("Cooldown State")]
    private List<SubJumpScareItemCooldownDebugInfo> ActiveItemCooldowns
    {
        get
        {
            if (_subJumpScareService == null)
            {
                return new List<SubJumpScareItemCooldownDebugInfo>();
            }

            return _subJumpScareService.GetDebugItemCooldowns();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _mainJumpScareService = new MainJumpScareService(this);
        _subJumpScareService = new SubJumpScareService(
            this,
            _playerController,
            _tensionManager,
            _fakeEnemyJumpScareExecutor,
            _postProcessJumpScareExecutor,
            _soundSubJumpScareExecutor,
            _database);

        _mainJumpScareService.RegisterSceneMainJumpScares();

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        _subscriptions = new CompositeSubscription();
        _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(OnSonarActive));
    }

    private void Update()
    {
        if (!_activeSwitch)
        {
            return;
        }
        
        _subJumpScareService?.Tick(
            Time.deltaTime,
            _usePeriodicTick,
            _periodicInterval,
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false);
    }

    private void OnDestroy()
    {
        _subscriptions?.Dispose();
        _subJumpScareService?.Dispose();

        if (_instance == this)
        {
            _instance = null;
        }
    }
    
    [Button]
    public void SetActive(bool tf)
    {
        _activeSwitch = tf;
    }

    public void ActiveFalse()
    {
        SetActive(false);
    }

    public void ActiveTrue()
    {
        SetActive(true);
    }

    private void OnSonarActive(SonarScanStartedRawEvent data)
    {
        if (!CanTriggerJumpScare())
        {
            return;
        }

        _subJumpScareService?.TrySelectSonar(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false);
    }

    [ContextMenu("Debug/Register Main JumpScares")]
    public void RegisterSceneMainJumpScares()
    {
        _mainJumpScareService?.RegisterSceneMainJumpScares();
    }

    public bool TryExecuteMainJumpScare(string id)
    {
        if (!CanTriggerJumpScare())
        {
            return false;
        }

        if (_mainJumpScareService?.TryExecuteMainJumpScare(id) == true)
        {
            return true;
        }

        return false;
    }

    public void SetMainJumpScareCanActive(string id, bool canActive)
    {
        _mainJumpScareService?.SetMainJumpScareCanActive(id, canActive);
    }

    public void NotifyMainJumpScareFinished(string id)
    {
        _mainJumpScareService?.NotifyMainJumpScareFinished(id);
    }

    [ContextMenu("Debug/Try Select Periodic")]
    public void TrySelectPeriodic()
    {
        if (!CanTriggerJumpScare())
        {
            return;
        }

        _subJumpScareService?.TrySelectPeriodic(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false);
    }

    [ContextMenu("Debug/Try Select Sonar")]
    public void TrySelectSonar()
    {
        if (!CanTriggerJumpScare())
        {
            return;
        }

        _subJumpScareService?.TrySelectSonar(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false);
    }

    private bool CanTriggerJumpScare()
    {
        return _activeSwitch;
    }
}
