using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private static StatisticsManager _instance;
    public static StatisticsManager Instance => _instance;

    [Title("Runtime")]
    [FoldoutGroup("View", expanded: true)]
    [ShowInInspector]
    [ReadOnly]
    private CurrentRunStatistics _currentRun;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    private PersistentStatistics _persistent;

    [Title("Debug Command")]
    [FoldoutGroup("Command", expanded: false)]
    [LabelText("Current Sonar")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunSonarCount;

    [FoldoutGroup("Command")]
    [LabelText("Current Lidar")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunLidarCount;

    [FoldoutGroup("Command")]
    [LabelText("Current AI")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunAiQuestionCount;

    [FoldoutGroup("Command")]
    [LabelText("Persistent Sonar")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentSonarCount;

    [FoldoutGroup("Command")]
    [LabelText("Persistent Lidar")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentLidarCount;

    [FoldoutGroup("Command")]
    [LabelText("Persistent AI")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentAiQuestionCount;

    [FoldoutGroup("Command")]
    [LabelText("Log Item ID")]
    [MinValue(1)]
    [SerializeField] private int _debugLogItemId = 1;

    private IStatisticsRepository _repository;

    public CurrentRunStatistics CurrentRun => _currentRun;
    public PersistentStatistics Persistent => _persistent;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _repository = new StatisticsRepository();
        _persistent = _repository.Load();

        if (_currentRun == null)
        {
            _currentRun = new CurrentRunStatistics();
        }

        _currentRun.Begin();
        SyncDebugFields();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    [Button]
    public void BeginRun()
    {
        if (_currentRun == null)
        {
            _currentRun = new CurrentRunStatistics();
        }

        _currentRun.Begin();
        SyncDebugFields();
    }

    public void RecordSonarUsed()
    {
        if (_currentRun == null)
        {
            return;
        }

        _currentRun.IncrementSonarUseCount();
        SyncDebugFields();
    }

    public void RecordLidarRestored()
    {
        if (_currentRun == null)
        {
            return;
        }

        _currentRun.IncrementLidarRestoreCount();
        SyncDebugFields();
    }

    public void RecordAiQuestionUsed()
    {
        if (_currentRun == null)
        {
            return;
        }

        _currentRun.IncrementAiQuestionCount();
        SyncDebugFields();
    }

    public void RecordLogCollected(int itemId)
    {
        if (_persistent == null)
        {
            return;
        }

        bool added = _persistent.TryAddCollectedLogItemId(itemId);
        if (added == false)
        {
            return;
        }

        SavePersistent();
        SyncDebugFields();
    }

    public bool HasCollectedLog(int itemId)
    {
        if (_persistent == null)
        {
            return false;
        }

        for (int index = 0; index < _persistent.CollectedLogItemIds.Count; index++)
        {
            if (_persistent.CollectedLogItemIds[index] == itemId)
            {
                return true;
            }
        }

        return false;
    }

    public int GetCollectedLogCount()
    {
        if (_persistent == null)
        {
            return 0;
        }

        return _persistent.GetCollectedLogCount();
    }

    public void FinalizeRunAndAccumulate()
    {
        if (_currentRun == null)
        {
            return;
        }

        _currentRun.MarkEnded();

        if (_persistent == null)
        {
            _persistent = new PersistentStatistics();
        }

        _persistent.AddSonarUseCount(_currentRun.SonarUseCount);
        _persistent.AddLidarRestoreCount(_currentRun.LidarRestoreCount);
        _persistent.AddAiQuestionCount(_currentRun.AiQuestionCount);

        SavePersistent();
        SyncDebugFields();
    }

    public void SavePersistent()
    {
        if (_repository == null || _persistent == null)
        {
            return;
        }

        _repository.Save(_persistent);
    }

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Current Run Active")]
    public bool DebugHasCurrentRun => _currentRun != null;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Persistent Ready")]
    public bool DebugHasPersistent => _persistent != null;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Collected Log Count")]
    public int DebugCollectedLogCount => GetCollectedLogCount();

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    [ListDrawerSettings(IsReadOnly = true, DraggableItems = false, Expanded = true)]
    [LabelText("Collected Log Item IDs")]
    public int[] DebugCollectedLogItemIds
    {
        get
        {
            if (_persistent == null || _persistent.CollectedLogItemIds == null)
            {
                return Array.Empty<int>();
            }

            int count = _persistent.CollectedLogItemIds.Count;
            int[] values = new int[count];

            for (int index = 0; index < count; index++)
            {
                values[index] = _persistent.CollectedLogItemIds[index];
            }

            return values;
        }
    }

    [FoldoutGroup("Command")]
    [Button("Sync Debug Fields", ButtonSizes.Medium)]
    private void DebugSyncFields()
    {
        SyncDebugFields();
    }

    [FoldoutGroup("Command")]
    [Button("Apply Current Run Counts", ButtonSizes.Medium)]
    private void DebugApplyCurrentRunCounts()
    {
        if (_currentRun == null)
        {
            _currentRun = new CurrentRunStatistics();
            _currentRun.Begin();
        }

        _currentRun.SetCounts(
            _debugCurrentRunSonarCount,
            _debugCurrentRunLidarCount,
            _debugCurrentRunAiQuestionCount);

        SyncDebugFields();
    }

    [FoldoutGroup("Command")]
    [Button("Apply Persistent Counts", ButtonSizes.Medium)]
    private void DebugApplyPersistentCounts()
    {
        if (_persistent == null)
        {
            _persistent = new PersistentStatistics();
        }

        _persistent.SetCounts(
            _debugPersistentSonarCount,
            _debugPersistentLidarCount,
            _debugPersistentAiQuestionCount);

        SavePersistent();
        SyncDebugFields();
    }

    [FoldoutGroup("Command")]
    [Button("Add Test Log", ButtonSizes.Medium)]
    private void DebugAddCollectedLog()
    {
        RecordLogCollected(_debugLogItemId);
    }

    [FoldoutGroup("Command")]
    [Button("Remove Test Log", ButtonSizes.Medium)]
    private void DebugRemoveCollectedLog()
    {
        if (_persistent == null)
        {
            return;
        }

        if (_persistent.RemoveCollectedLogItemId(_debugLogItemId) == false)
        {
            return;
        }

        SavePersistent();
        SyncDebugFields();
    }

    [FoldoutGroup("Command")]
    [Button("Clear Collected Logs", ButtonSizes.Medium)]
    private void DebugClearCollectedLogs()
    {
        if (_persistent == null)
        {
            return;
        }

        _persistent.ClearCollectedLogItemIds();
        SavePersistent();
        SyncDebugFields();
    }

    [FoldoutGroup("Command")]
    [Button("Finalize Run And Save", ButtonSizes.Medium)]
    private void DebugFinalizeRunAndAccumulate()
    {
        FinalizeRunAndAccumulate();
    }

    [FoldoutGroup("Command")]
    [Button("Reload Persistent From Save", ButtonSizes.Medium)]
    private void DebugReloadPersistent()
    {
        if (_repository == null)
        {
            _repository = new StatisticsRepository();
        }

        _persistent = _repository.Load();
        SyncDebugFields();
    }

    [Button("Reset Persistent Statistics")]
    [FoldoutGroup("Command")]
    public void ResetPersistent()
    {
        if (_repository == null)
        {
            return;
        }

        _repository.Reset();
        _persistent = new PersistentStatistics();
        SyncDebugFields();
    }

    [Button("Reset Current Run")]
    [FoldoutGroup("Command")]
    public void ResetCurrentRun()
    {
        BeginRun();
    }

    private void SyncDebugFields()
    {
        _debugCurrentRunSonarCount = _currentRun != null ? _currentRun.SonarUseCount : 0;
        _debugCurrentRunLidarCount = _currentRun != null ? _currentRun.LidarRestoreCount : 0;
        _debugCurrentRunAiQuestionCount = _currentRun != null ? _currentRun.AiQuestionCount : 0;

        _debugPersistentSonarCount = _persistent != null ? _persistent.SonarUseCount : 0;
        _debugPersistentLidarCount = _persistent != null ? _persistent.LidarRestoreCount : 0;
        _debugPersistentAiQuestionCount = _persistent != null ? _persistent.AiQuestionCount : 0;
    }
}
