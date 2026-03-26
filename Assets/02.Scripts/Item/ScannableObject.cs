using System;
using UnityEngine;
using UnityEngine.Events;

public class ScannableObject : MonoBehaviour,IScannableObject
{
    [Header("Required References")]
    [SerializeField] private ScanProgressSetting _settings;
    [SerializeField] private StateKeySO _scanCompleteStateKey;
    
    private IItemInstance _itemBinder;
    private ScanProgress _progress;
    private ScanStateMachine _fsm;
    
    public bool IsProgressComplete => _progress != null && _progress.IsActivated;
    public float CurrentProgress => _progress != null ? _progress.CurrentProgress : 0.0f;
    public float ProgressRatio => _progress != null ? _progress.ProgressRatio : 0.0f;
    public EScannableState State => _fsm.CurrentStateType;

    
    public event Action<float> OnScanProgressChanged; //ratio전달
    public event Action OnScanComplete;
    
    [Header("Scene Event")]
    public UnityEvent ScanStartEvent;
    public UnityEvent ScanEndEvent;
    public UnityEvent ScanCompletEvent;
    
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
        
        _itemBinder = GetComponentInParent<InstanceView>();
        
        if (_progress != null)
        {
            _progress.OnProgressChanged -= HandleProgressChanged;
            _progress.OnActivated -= OnScanCompleted;
        }

        _progress = new ScanProgress(_settings);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += OnScanCompleted;

        _fsm = new ScanStateMachine(this);
        ChangeState(EScannableState.Default, true);
    }

    [ContextMenu("Reset")]
    public void ResetAll()
    {
        _progress?.Reset();
        ChangeState(EScannableState.Default, true);
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
    }

    public void OnScanStopped()
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanStopped();
        ScanEndEvent?.Invoke();
    }
   
    public void OnScanCompleted()
    {
        if (_itemBinder != null)
        {
            InstanceView instanceView = _itemBinder as InstanceView;
            ItemInstance itemInstance = instanceView != null ? instanceView.EnsureItemInstance() : _itemBinder.ItemInstance;
            if (itemInstance?.State != null && _scanCompleteStateKey != null)
            {
                itemInstance.State.SetBool(_scanCompleteStateKey, true);
            }
        }
        
        OnScanComplete?.Invoke();
        ScanCompletEvent?.Invoke();
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

        ChangeState(EScannableState.OnCompleted, true);
        
        OnScanProgressChanged?.Invoke(1);
        OnScanComplete?.Invoke();
    }

    public void ChangeState(EScannableState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }

    public void ChangeState(EScannableState nextState)
    {
        ChangeState(nextState, false);
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

    public void ReduceProgressByReturn(float deltaTime)
    {
        _progress.Reduce(_settings.ReturnSpeed * deltaTime);
        if (CurrentProgress <= 0.0f)
        {
            ChangeState(EScannableState.Default);
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

        ChangeState(EScannableState.OnCompleted);
        return true;
    }

    public void PauseScanning()
    {
        if (IsProgressComplete)
        {
            return;
        }

        ChangeState(EScannableState.OnHold);
    }
}
