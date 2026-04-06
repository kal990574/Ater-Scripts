using Sirenix.OdinInspector;
using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    private static JumpScareManager _instance;

    public static JumpScareManager Instance => _instance;

    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TensionManager _tensionManager;
    [SerializeField] private Transform _playerRootTransform;
    [SerializeField] private Transform _playerCameraTransform;

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

    [Header("Runtime State")]
    [SerializeField] private bool _isImportantVoicePlaying;
    [SerializeField] private bool _isSonarAvailable = true;
    [SerializeField] private bool _canPlaceFakeEnemyThisAttempt = true;

    [Header("Sub Debug")]
    [SerializeField] private bool _enableLog = true;
    [SerializeField] private bool _enableSelectionLog = true;
    [SerializeField] private bool _enableGuaranteeLog = true;

    [Header("Main Debug")]
    [SerializeField] private bool _enableDebugLog = true;
    [SerializeField] private bool _includeInactiveOnRegister = true;

    private CompositeSubscription _subscriptions;
    private MainJumpScareService _mainJumpScareService;
    private SubJumpScareService _subJumpScareService;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _mainJumpScareService = new MainJumpScareService(this, _enableDebugLog, _includeInactiveOnRegister);
        _subJumpScareService = new SubJumpScareService(
            this,
            _playerController,
            _tensionManager,
            _playerRootTransform,
            _playerCameraTransform,
            _fakeEnemyJumpScareExecutor,
            _postProcessJumpScareExecutor,
            _soundSubJumpScareExecutor,
            _database,
            _enableLog,
            _enableSelectionLog,
            _enableGuaranteeLog);

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
        _subJumpScareService?.Tick(
            Time.deltaTime,
            _usePeriodicTick,
            _periodicInterval,
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false,
            _isImportantVoicePlaying,
            _isSonarAvailable,
            _canPlaceFakeEnemyThisAttempt);
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

    private void OnSonarActive(SonarScanStartedRawEvent data)
    {
        _subJumpScareService?.TrySelectSonar(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false,
            _isImportantVoicePlaying,
            _isSonarAvailable,
            _canPlaceFakeEnemyThisAttempt);
    }

    [ContextMenu("Debug/Register Main JumpScares")]
    public void RegisterSceneMainJumpScares()
    {
        _mainJumpScareService?.RegisterSceneMainJumpScares();
    }

    public void ExecuteMainJumpScare(string id)
    {
        _mainJumpScareService?.ExecuteMainJumpScare(id);
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
        _subJumpScareService?.TrySelectPeriodic(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false,
            _isImportantVoicePlaying,
            _isSonarAvailable,
            _canPlaceFakeEnemyThisAttempt);
    }

    [ContextMenu("Debug/Try Select Sonar")]
    public void TrySelectSonar()
    {
        _subJumpScareService?.TrySelectSonar(
            _mainJumpScareService != null && _mainJumpScareService.IsAnyMainJumpScarePlaying,
            false,
            _isImportantVoicePlaying,
            _isSonarAvailable,
            _canPlaceFakeEnemyThisAttempt);
    }
}
