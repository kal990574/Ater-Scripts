using Sirenix.OdinInspector;
using UnityEngine;

public class StatisticsRunLifecycleBridge : MonoBehaviour
{
    private readonly GameEventPublisher _publisher = new GameEventPublisher();
    private bool _isEnded;

    private void Awake()
    {
        _publisher.SetSource(this);
    }

    private void Start()
    {
        StatisticsManager manager = StatisticsManager.Instance;
        if (manager != null)
        {
            manager.BeginRun();
        }

        _isEnded = false;
    }

    [Button]
    public void NotifyClear()
    {
        PublishRunEnded(EStatisticsRunEndReason.Clear);
    }
    [Button]
    public void NotifyGameOver()
    {
        PublishRunEnded(EStatisticsRunEndReason.GameOver);
    }
    [Button]
    public void NotifyQuitToMenu()
    {
        PublishRunEnded(EStatisticsRunEndReason.QuitToMenu);
    }

    private void PublishRunEnded(EStatisticsRunEndReason endReason)
    {
        if (_isEnded == true)
        {
            return;
        }

        StatisticsManager manager = StatisticsManager.Instance;
        if (manager == null || manager.CurrentRun == null)
        {
            return;
        }

        manager.CurrentRun.MarkEnded();
        StatisticsRunSummary summary = new StatisticsRunSummary(manager.CurrentRun);

        _publisher.TryPublish(context => new StatisticsRunEndedRawEvent(context, summary, endReason));
        _isEnded = true;
    }
}
