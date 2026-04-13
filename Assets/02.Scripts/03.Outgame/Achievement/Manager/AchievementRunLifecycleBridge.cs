// AchievementRunLifecycleBridge.cs
using UnityEngine;

public class AchievementRunLifecycleBridge : MonoBehaviour
{
    private readonly GameEventPublisher _publisher = new GameEventPublisher();

    private bool _isEnded;

    private void Awake()
    {
        _publisher.SetSource(this);
    }

    private void Start()
    {
        AchievementManager manager = AchievementManager.Instance;
        if (manager != null)
        {
            manager.BeginRun();
        }

        _isEnded = false;
    }

    public void NotifyClear()
    {
        PublishRunEnded(EAchievementRunEndReason.Clear);
    }

    public void NotifyGameOver()
    {
        PublishRunEnded(EAchievementRunEndReason.GameOver);
    }

    public void NotifyQuitToMenu()
    {
        PublishRunEnded(EAchievementRunEndReason.QuitToMenu);
    }

    private void PublishRunEnded(EAchievementRunEndReason endReason)
    {
        if (_isEnded == true)
        {
            return;
        }

        AchievementManager manager = AchievementManager.Instance;
        if (manager == null || manager.CurrentRun == null)
        {
            return;
        }

        manager.CurrentRun.MarkEnded();

        AchievementRunSummary summary = new AchievementRunSummary(manager.CurrentRun);

        _publisher.TryPublish(context => new AchievementRunEndedRawEvent(context, summary, endReason));
        _isEnded = true;
    }
}