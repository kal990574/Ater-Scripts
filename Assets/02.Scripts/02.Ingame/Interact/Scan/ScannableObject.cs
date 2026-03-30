using System;
using UnityEngine;
using UnityEngine.Events;

public class ScannableObject : GameEventPublisher, IScannable
{
    [Header("Required References")]
    [SerializeField] private ScanProgressSetting _settings;
    [SerializeField] private Rigidbody _targetRigidbody;
    
    private IRuntimeView _instance;
    private ScanProgress _progress;
    private ScanFSM _fsm;
    private ScannableQTEInvoker _qteInvoker;
    
    public bool IsProgressComplete => _progress != null && _progress.IsActivated;
    public float CurrentProgress => _progress != null ? _progress.CurrentProgress : 0.0f;
    public float ProgressRatio => _progress != null ? _progress.ProgressRatio : 0.0f;
    public EScanState State => _fsm.CurrentStateType;

    
    public event Action<float> OnScanProgressChanged; //ratio전달
    public event Action OnScanComplete;
    
    [Header("Scene Event")]
    public UnityEvent ScanStartEvent;
    public UnityEvent ScanEndEvent;
    public UnityEvent ScanCompleteEvent;
    
    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        if (_progress != null)
        {
            _progress.OnProgressChanged -= HandleProgressChanged;
            _progress.OnActivated -= OnScanCompleted;
        }
    }

    private void Update()
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.Tick(Time.deltaTime);
    }

    public void Init()
    {
        if (_settings == null)
        {
            Debug.LogError($"[{nameof(ScannableObject)}] {nameof(ScanProgressSetting)} is missing.", this);
            enabled = false;
            return;
        }
        
        if (_targetRigidbody == null)
        {
            _targetRigidbody = GetComponentInParent<Rigidbody>();
        }

        if (_instance == null)
        {
            _instance = GetComponentInParent<IRuntimeView>();
        }
        
        _qteInvoker = GetComponent<ScannableQTEInvoker>();
        if (_qteInvoker == null)
        {
            _qteInvoker = GetComponentInChildren<ScannableQTEInvoker>();
        }
        
        if (_progress != null)
        {
            _progress.OnProgressChanged -= HandleProgressChanged;
            _progress.OnActivated -= OnScanCompleted;
        }

        _progress = new ScanProgress(_settings);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += OnScanCompleted;

        _fsm = new ScanFSM(this);
        ChangeState(EScanState.Default, true);
        ApplyPhysicsState(false);
    }

    [ContextMenu("Reset")]
    public void ResetAll()
    {
        _progress?.Reset();
        ChangeState(EScanState.Default, true);
        ApplyPhysicsState(false);
    }

    public void OnScanStarted()
    {
        ScanStartEvent?.Invoke();
    }

    public void OnScanning(float deltaTime)
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanning(deltaTime);
        _qteInvoker?.HandleScanning(deltaTime);
    }

    public void OnScanStopped()
    {
        if (IsProgressComplete)
        {
            return;
        }

        bool shouldNotifyScanLost = _qteInvoker == null || _qteInvoker.HandleScanStopped();
        if (shouldNotifyScanLost)
        {
            _fsm.OnScanStopped();
            ScanEndEvent?.Invoke();
        }
    }
   
    public void OnScanCompleted()
    {
        ApplyPhysicsState(true);
        
        OnScanComplete?.Invoke();
        ScanCompleteEvent?.Invoke();
        
        if (TryGetHub(out GameEventHub hub) == false)
        {
            Debug.LogWarning("[PickupEventEmitter] GameEventHub가 존재하지 않습니다.");
            return;
        }

        if (_instance == null)
        {
            Debug.LogWarning($"[{nameof(ScannableObject)}] {nameof(IRuntimeView)} is missing. Event publish skipped.", this);
            return;
        }
        
        GameEventContext eventContext = CreateContext();
        ScanCompleteEvent gameCompleteEvent = new ScanCompleteEvent(
            eventContext, 
            _instance.InstanceId, 
            gameObject.name);

        hub.Publish(in gameCompleteEvent);
    }
    
    public void AddProgress(float amount)
    {
        _progress.Add(amount);
    }

    public void ReduceProgress(float amount)
    {
        _progress.Reduce(amount);
    }
    
    private void HandleProgressChanged(float ratio)
    {
        OnScanProgressChanged?.Invoke(ratio);
    }

    public void ChangeState(EScanState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }

    public void ChangeState(EScanState nextState)
    {
        ChangeState(nextState, false);
    }

    public void ReduceProgressByReturn(float deltaTime)
    {
        _progress.Reduce(_settings.ReturnSpeed * deltaTime);
        if (CurrentProgress <= 0.0f)
        {
            ChangeState(EScanState.Default);
        }
    }

    public void ApplyScanProgress(float deltaTime)
    {
        AddProgress(deltaTime);
    }

    public bool TryTransitToCompleted()
    {
        if (IsProgressComplete == false)
        {
            return false;
        }

        ChangeState(EScanState.OnCompleted);
        return true;
    }

    public void PauseScanning()
    {
        if (IsProgressComplete)
        {
            return;
        }

        ChangeState(EScanState.OnHold);
    }
    
    [ContextMenu("Force")]
    public void ForceScanComplete()
    {
        if (_progress == null || _fsm == null)
        {
            Init();
        }

        if (IsProgressComplete == false)
        {
            AddProgress(_settings.RequiredScanTime);
        }

        ChangeState(EScanState.OnCompleted, true);
        ApplyPhysicsState(true);
        
        OnScanProgressChanged?.Invoke(1);
        OnScanComplete?.Invoke();
    }

    private void ApplyPhysicsState(bool isScanComplete)
    {
        if (_targetRigidbody == null)
        {
            return;
        }

        _targetRigidbody.useGravity = isScanComplete;
        _targetRigidbody.isKinematic = !isScanComplete;
    }
    
}
