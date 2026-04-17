public class OnScanCompletedInterpreter : SubTensionInterpreterBase
{
    public OnScanCompletedInterpreter(UnityEngine.Object source) : base(source)
    {
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<LidarScanTargetCompletedRawEvent>(PublishTensionEvent));
    }

    private void PublishTensionEvent(LidarScanTargetCompletedRawEvent data)
    {
        publisher.TryPublish(
            context => new OnTensionChangedEvent(
                context,
                TensionReasons.LidarScanCompleted,
                ETensionChannel.BaseTension,
                TensionDeltaConstants.LidarScanCompletedBase));
    }
}
