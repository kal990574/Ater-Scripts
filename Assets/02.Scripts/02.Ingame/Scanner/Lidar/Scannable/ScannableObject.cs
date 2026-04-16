using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
public class ScannableObject : MonoBehaviour, IScannable
{
    private const string ScanStateKey = "is_scan";
    private const string InteractLayerName = "Interact";

    [TabGroup("Inspector", "References")]
    [Required]
    [LabelText("Progress Settings")]
    [SerializeField] private ScanProgressSettingSO _setting;

    [TabGroup("Inspector", "References")]
    [LabelText("Active Rigidbody Switch")]
    [SerializeField] private bool _activePhysicsAfterScanComplete;

    [TabGroup("Inspector", "References")]
    [LabelText("Target Rigidbody")]
    [SerializeField, ShowIf(nameof(_activePhysicsAfterScanComplete))] private Rigidbody _targetRigidbody;

    private GameEventPublisher _eventPublisher;
    private IRuntimeView _runtimeView;
    private ScannableScanController _scanController;
    private ScannableQTEInvoker _qteInvoker;

    private int _interactLayer = -1;
    private bool _isScanCompletedApplied;

    public bool IsProgressComplete => _scanController != null && _scanController.IsProgressComplete;
    public float CurrentProgress => _scanController != null ? _scanController.CurrentProgress : 0.0f;
    public float ProgressRatio => _scanController != null ? _scanController.ProgressRatio : 0.0f;
    public EScanState State => _scanController == null ? EScanState.Default : _scanController.State;
    public IScannableQTEHandler QteHandler => _scanController;

    public event Action<float> OnScanProgressChanged;
    public event Action OnScanStart;
    public event Action OnScanEnd;
    public event Action OnScanComplete;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Scan Started")]
    public UnityEvent OnScanStartUnityEvent;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Scan Ended")]
    public UnityEvent OnScanEndUnityEvent;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Scan Completed")]
    public UnityEvent OnScanCompleteUnityEvent;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("State")]
    private EScanState DebugState => State;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Current Progress")]
    private float DebugCurrentProgress => CurrentProgress;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, ProgressBar(0f, 1f), LabelText("Progress Ratio")]
    private float DebugProgressRatio => ProgressRatio;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Runtime View")]
    private Object DebugRuntimeView => _runtimeView as Object;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("QTE Invoker")]
    private ScannableQTEInvoker DebugQteInvoker => _qteInvoker;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Completed State Applied")]
    private bool DebugIsCompletedApplied => _isScanCompletedApplied;

    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        _scanController?.Dispose();
    }

    private void Update()
    {
        _scanController?.Tick(Time.deltaTime);
    }

    public void Init()
    {
        if (_setting == null)
        {
            Debug.LogError($"[{nameof(ScannableObject)}] {nameof(ScanProgressSettingSO)} is missing.", this);
            enabled = false;
            return;
        }

        ResolveReferences();
        SetupEventPublisher();
        SetupScanController();
        ResetCompletionState();
    }

    [TabGroup("Inspector", "Debug")]
    [Button("Reset Scan", ButtonSizes.Medium)]
    [ContextMenu("Reset")]
    public void ResetAll()
    {
        _scanController?.Reset();
        ResetCompletionState();
    }

    public void OnScanStarted()
    {
        if (IsProgressComplete)
        {
            return;
        }

        OnScanStart?.Invoke();
        OnScanStartUnityEvent?.Invoke();
    }

    public void OnScanning(float deltaTime)
    {
        if (_scanController == null)
        {
            return;
        }

        _scanController.OnScanning(deltaTime);
        _qteInvoker?.HandleScanning(deltaTime);
    }

    public void OnScanStopped()
    {
        if (_scanController == null)
        {
            return;
        }

        bool shouldNotifyScanLost = _qteInvoker == null || _qteInvoker.HandleScanStopped();
        if (!shouldNotifyScanLost)
        {
            return;
        }

        _scanController.OnScanStopped();
        OnScanEnd?.Invoke();
        OnScanEndUnityEvent?.Invoke();
    }

    private void OnScanCompleted()
    {
        CompleteScan(notifyListeners: true, updateRuntimeState: true, publishGameEvent: true);
    }

    private void HandleProgressChanged(float ratio)
    {
        OnScanProgressChanged?.Invoke(ratio);
    }

    private void ApplyPhysicsState(bool isScanComplete)
    {
        if (!_activePhysicsAfterScanComplete || _targetRigidbody == null)
        {
            return;
        }

        _targetRigidbody.useGravity = isScanComplete;
        _targetRigidbody.isKinematic = !isScanComplete;
    }

    [TabGroup("Inspector", "Debug")]
    [Button("Force Complete", ButtonSizes.Medium)]
    [ContextMenu("Force")]
    public void ForceScanComplete()
    {
        EnsureScanControllerInitialized();
        _scanController?.ForceComplete();
    }

    public void ApplyRuntimeScanState(bool isCompleted)
    {
        if (isCompleted)
        {
            EnsureScanControllerInitialized();
            _scanController?.RestoreCompletedState();
            ApplyCompletedState();
            return;
        }

        ResetAll();
    }

    private void SetInteractable()
    {
        IRuntimeInteractObject[] interactObjects = GetComponentsInChildren<IRuntimeInteractObject>();
        foreach (IRuntimeInteractObject interactObject in interactObjects)
        {
            interactObject.SetActivate(true);
        }
    }

    private void ResolveReferences()
    {
        if (_activePhysicsAfterScanComplete && _targetRigidbody == null)
        {
            _targetRigidbody = GetComponentInParent<Rigidbody>();
        }

        if (_runtimeView == null)
        {
            _runtimeView = GetComponentInParent<IRuntimeView>();
        }

        if (_qteInvoker == null)
        {
            _qteInvoker = GetComponent<ScannableQTEInvoker>();
        }

        if (_qteInvoker == null)
        {
            _qteInvoker = GetComponentInChildren<ScannableQTEInvoker>();
        }
    }

    private void SetupEventPublisher()
    {
        if (_eventPublisher != null)
        {
            return;
        }

        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);
    }

    private void SetupScanController()
    {
        _scanController?.Dispose();
        _scanController = new ScannableScanController(_setting, HandleProgressChanged, OnScanCompleted);
    }

    private void EnsureScanControllerInitialized()
    {
        if (_scanController != null)
        {
            return;
        }

        SetupScanController();
    }

    private void ResetCompletionState()
    {
        _isScanCompletedApplied = false;
        _scanController?.ChangeState(EScanState.Default, true);
        ApplyPhysicsState(false);
    }

    private void CompleteScan(bool notifyListeners, bool updateRuntimeState, bool publishGameEvent)
    {
        bool wasCompleted = _isScanCompletedApplied;

        ApplyCompletedState();
        _scanController?.ChangeState(EScanState.OnCompleted, true);

        if (updateRuntimeState)
        {
            PersistCompletedState();
        }

        if (wasCompleted || !notifyListeners)
        {
            return;
        }

        OnScanComplete?.Invoke();
        OnScanCompleteUnityEvent?.Invoke();

        if (publishGameEvent)
        {
            _eventPublisher?.TryPublish(
                context => new LidarScanTargetCompletedRawEvent(context, this));
        }
    }

    private void ApplyCompletedState()
    {
        ApplyPhysicsState(true);
        SetInteractable();
        _isScanCompletedApplied = true;
    }

    private void PersistCompletedState()
    {
        if (_runtimeView?.RuntimeData?.State == null)
        {
            return;
        }

        _runtimeView.RuntimeData.State.SetBool(ScanStateKey, true);
    }
}
