using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private static StatisticsManager _instance;
    public static StatisticsManager Instance => _instance;
    private readonly GameEventPublisher _publisher = new GameEventPublisher();

    [Title("Runtime")]
    [FoldoutGroup("View", expanded: true)]
    [ShowInInspector]
    [ReadOnly]
    private CurrentRunStatistics _currentRun;

    [FoldoutGroup("View")]
    [ShowInInspector]
    [ReadOnly]
    private PersistentStatistics _persistent;

    

    private IStatisticsRepository _repository;
    private bool _isRunEnded;
    private int _committedRunSonarCount;
    private int _committedRunLidarCount;
    private int _committedRunAiQuestionCount;

    public CurrentRunStatistics CurrentRun => _currentRun;
    public PersistentStatistics Persistent => _persistent;
    public int TotalSonarUseCount => GetTotalSonarUseCount();
    public int TotalLidarRestoreCount => GetTotalLidarRestoreCount();
    public int TotalAiQuestionCount => GetTotalAiQuestionCount();

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

        _publisher.SetSource(this);
        _repository = new StatisticsPlayerPrefsRepository();
        _persistent = _repository.Load();

        if (_currentRun == null)
        {
            _currentRun = new CurrentRunStatistics();
        }

        SyncDebugFields();
    }

    private void Start()
    {
        BeginRun();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    [FoldoutGroup("EventCommand")]
    [Button("BeginRun", ButtonSizes.Medium)]
    public void BeginRun()
    {
        if (_currentRun == null)
        {
            _currentRun = new CurrentRunStatistics();
        }

        _currentRun.Begin();
        _isRunEnded = false;
        _committedRunSonarCount = 0;
        _committedRunLidarCount = 0;
        _committedRunAiQuestionCount = 0;
        SyncDebugFields();
        _publisher.TryPublish(context => new InGameRunStartedRawEvent(context));
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

    public int GetTotalSonarUseCount()
    {
        int persistentValue = _persistent != null ? _persistent.SonarUseCount : 0;
        int currentRunDelta = _currentRun != null
            ? Mathf.Max(0, _currentRun.SonarUseCount - _committedRunSonarCount)
            : 0;
        return persistentValue + currentRunDelta;
    }

    public int GetTotalLidarRestoreCount()
    {
        int persistentValue = _persistent != null ? _persistent.LidarRestoreCount : 0;
        int currentRunDelta = _currentRun != null
            ? Mathf.Max(0, _currentRun.LidarRestoreCount - _committedRunLidarCount)
            : 0;
        return persistentValue + currentRunDelta;
    }

    public int GetTotalAiQuestionCount()
    {
        int persistentValue = _persistent != null ? _persistent.AiQuestionCount : 0;
        int currentRunDelta = _currentRun != null
            ? Mathf.Max(0, _currentRun.AiQuestionCount - _committedRunAiQuestionCount)
            : 0;
        return persistentValue + currentRunDelta;
    }

    public void FinalizeRunAndAccumulate()
    {
        if (_currentRun == null)
        {
            return;
        }

        CommitCurrentRunToPersistent();
        _currentRun.MarkEnded();
        SyncDebugFields();
    }

    [FoldoutGroup("CurrentCommand")]
    [Button("CommitCurrentRunToPersistent", ButtonSizes.Medium)]
    public void CommitCurrentRunToPersistent()
    {
        if (_currentRun == null)
        {
            return;
        }

        if (_persistent == null)
        {
            _persistent = new PersistentStatistics();
        }

        int sonarDelta = Mathf.Max(0, _currentRun.SonarUseCount - _committedRunSonarCount);
        int lidarDelta = Mathf.Max(0, _currentRun.LidarRestoreCount - _committedRunLidarCount);
        int aiDelta = Mathf.Max(0, _currentRun.AiQuestionCount - _committedRunAiQuestionCount);

        if (sonarDelta == 0 && lidarDelta == 0 && aiDelta == 0)
        {
            return;
        }

        _persistent.AddSonarUseCount(sonarDelta);
        _persistent.AddLidarRestoreCount(lidarDelta);
        _persistent.AddAiQuestionCount(aiDelta);

        _committedRunSonarCount = _currentRun.SonarUseCount;
        _committedRunLidarCount = _currentRun.LidarRestoreCount;
        _committedRunAiQuestionCount = _currentRun.AiQuestionCount;

        SavePersistent();
        SyncDebugFields();
    }

    [FoldoutGroup("EventCommand")]
    [Button("Chapter Clear event", ButtonSizes.Medium)]
    public void PublishChapterClear(int clearedChapter)
    {
        _publisher.TryPublish(context => new ChapterClearedRawEvent(context, clearedChapter));
    }
    
    [FoldoutGroup("EventCommand")]
    [Button("clear event", ButtonSizes.Medium)]
    public void NotifyClear()
    {
        EndRun(EStatisticsRunEndReason.Clear);
    }

    [FoldoutGroup("EventCommand")]
    [Button("gameover event", ButtonSizes.Medium)]
    public void NotifyGameOver()
    {
        EndRun(EStatisticsRunEndReason.GameOver);
    }
    
    [FoldoutGroup("EventCommand")]
    [Button("quit event", ButtonSizes.Medium)]
    public void NotifyQuitToMenu()
    {
        EndRun(EStatisticsRunEndReason.QuitToMenu);
    }

    public void EndRun(EStatisticsRunEndReason endReason)
    {
        if (_isRunEnded == true)
        {
            return;
        }

        if (_currentRun == null)
        {
            return;
        }

        _currentRun.MarkEnded();
        StatisticsRunSummary summary = new StatisticsRunSummary(_currentRun);
        _publisher.TryPublish(context => new InGameRunEndedRawEvent(context, summary, endReason));
        FinalizeRunAndAccumulate();
        _isRunEnded = true;
        BeginRun();
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
    
    [FoldoutGroup("CurrentCommand", expanded: false)]
    [LabelText("Current Sonar")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunSonarCount;

    [FoldoutGroup("CurrentCommand")]
    [LabelText("Current Lidar")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunLidarCount;

    [FoldoutGroup("CurrentCommand")]
    [LabelText("Current AI")]
    [MinValue(0)]
    [SerializeField] private int _debugCurrentRunAiQuestionCount;
    
    [FoldoutGroup("CurrentCommand")]
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

    
    [FoldoutGroup("PersistentCommand")]
    [LabelText("Persistent Sonar")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentSonarCount;

    [FoldoutGroup("PersistentCommand")]
    [LabelText("Persistent Lidar")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentLidarCount;

    [FoldoutGroup("PersistentCommand")]
    [LabelText("Persistent AI")]
    [MinValue(0)]
    [SerializeField] private int _debugPersistentAiQuestionCount;
    [FoldoutGroup("PersistentCommand")]
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
    
    [FoldoutGroup("PersistentCommand")]
    [Button("Reload Persistent From Save", ButtonSizes.Medium)]
    private void DebugReloadPersistent()
    {
        if (_repository == null)
        {
            _repository = new StatisticsPlayerPrefsRepository();
        }

        _persistent = _repository.Load();
        SyncDebugFields();
    }

    [FoldoutGroup("PersistentCommand")]
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


    [FoldoutGroup("LogCommand")]
    [LabelText("Log Item ID")]
    [MinValue(1)]
    [SerializeField] private int _debugLogItemId = 1;
    [FoldoutGroup("LogCommand")]
    [Button("Add Test Log", ButtonSizes.Medium)]
    private void DebugAddCollectedLog()
    {
        RecordLogCollected(_debugLogItemId);
    }
    [FoldoutGroup("LogCommand")]
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
    
    [FoldoutGroup("LogCommand")]
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
    [Button("Sync Debug Fields", ButtonSizes.Medium)]
    private void DebugSyncFields()
    {
        SyncDebugFields();
    }
    
    [FoldoutGroup("Command")]
    [Button("Finalize Run And Save", ButtonSizes.Medium)]
    private void DebugFinalizeRunAndAccumulate()
    {
        FinalizeRunAndAccumulate();
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
