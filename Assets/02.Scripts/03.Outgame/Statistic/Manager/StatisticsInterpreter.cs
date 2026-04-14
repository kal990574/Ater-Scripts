using UnityEngine;

public class StatisticsInterpreter : MonoBehaviour, IStatisticsInterpreter
{
    private readonly GameEventPublisher _publisher = new GameEventPublisher();
    private CompositeSubscription _subscriptions;

    private void Awake()
    {
        _publisher.SetSource(this);
    }

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
        _publisher.TryPublish(context => new StatisticsSonarUsedEvent(context));
    }

    private void OnLidarCompleted(LidarScanTargetCompletedRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordLidarRestored();
        _publisher.TryPublish(context => new StatisticsLidarRestoredEvent(context));
    }

    private void OnAiHintAnswered(AiHintAnsweredRawEvent rawEvent)
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null)
        {
            return;
        }

        manager.RecordAiQuestionUsed();
        _publisher.TryPublish(context => new StatisticsAiQuestionEvent(context));
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
        _publisher.TryPublish(context => new StatisticsLogCollectedEvent(context, rawEvent.ItemId));
    }
}
