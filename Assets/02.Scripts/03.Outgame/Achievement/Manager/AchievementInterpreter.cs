using UnityEngine;

public class AchievementInterpreter : MonoBehaviour
{
    [SerializeField] private AchievementManager _achievementManager;

    private CompositeSubscription _subscriptions;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        DisposeSubscriptions();
    }

    private void SubscribeEvents()
    {
        DisposeSubscriptions();

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return;
        }

        if (_achievementManager == null)
        {
            Debug.LogWarning("[AchievementInterpreter] AchievementManager reference is null.");
            return;
        }

        _subscriptions = new CompositeSubscription();

        _subscriptions.Add(hub.Subscribe<SonarScanStartedRawEvent>(OnSonarScanStarted));
        _subscriptions.Add(hub.Subscribe<LidarScanTargetCompletedRawEvent>(OnLidarScanTargetCompleted));
    }

    private void DisposeSubscriptions()
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
        AchievementSonarUsedEvent achievementEvent = new AchievementSonarUsedEvent(rawEvent.Context);
        _achievementManager.HandleEvent(achievementEvent);
    }

    private void OnLidarScanTargetCompleted(LidarScanTargetCompletedRawEvent rawEvent)
    {
        AchievementLidarTargetCompletedEvent achievementEvent = new AchievementLidarTargetCompletedEvent(rawEvent.Context);
        _achievementManager.HandleEvent(achievementEvent);
    }

    public void NotifyAiQuestionUsed(GameEventContext context)
    {
        if (_achievementManager == null)
        {
            return;
        }

        AchievementAiQuestionUsedEvent achievementEvent = new AchievementAiQuestionUsedEvent(context);
        _achievementManager.HandleEvent(achievementEvent);
    }

    public void NotifyEndingReached(GameEventContext context)
    {
        if (_achievementManager == null)
        {
            return;
        }

        AchievementEndingReachedEvent achievementEvent = new AchievementEndingReachedEvent(context);
        _achievementManager.HandleEvent(achievementEvent);
    }
}
