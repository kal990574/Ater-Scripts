using UnityEngine;

public class StatisticsInterpreter : MonoBehaviour, IStatisticsInterpreter
{
    private CompositeSubscription _subscriptions;

    private void OnEnable()
    {
        Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        if (_subscriptions != null)
        {
            return;
        }

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        _subscriptions = new CompositeSubscription();
        _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(OnSonarScanStarted));
        _subscriptions.Add(hub.Subscribe<LidarScanTargetCompletedRawEvent>(OnLidarCompleted));
        _subscriptions.Add(hub.Subscribe<AiHintAnsweredRawEvent>(OnAiHintAnswered));
        _subscriptions.Add(hub.Subscribe<ItemAcquiredRawEvent>(OnItemAcquired));
        _subscriptions.Add(hub.Subscribe<StatisticsRunEndedRawEvent>(OnRunEnded));
    }

    public void Disable()
    {
        if (_subscriptions == null)
        {
            return;
        }

        _subscriptions.Dispose();
        _subscriptions = null;
    }

    private void OnSonarScanStarted(SonarScanStartedRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordSonarUsed();
    }

    private void OnLidarCompleted(LidarScanTargetCompletedRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordLidarRestored();
    }

    private void OnAiHintAnswered(AiHintAnsweredRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordAiQuestionUsed();
    }

    private void OnItemAcquired(ItemAcquiredRawEvent rawEvent)
    {
        if (rawEvent.ItemType != EItemType.Log)
        {
            return;
        }

        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordLogCollected(rawEvent.ItemId);
    }

    private void OnRunEnded(StatisticsRunEndedRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.FinalizeRunAndAccumulate();
        manager.BeginRun();
    }
}
