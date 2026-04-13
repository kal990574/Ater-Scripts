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

    
}
