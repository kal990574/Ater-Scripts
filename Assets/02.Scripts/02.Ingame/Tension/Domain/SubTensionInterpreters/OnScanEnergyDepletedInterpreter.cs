public class OnScanEnergyDepletedInterpreter : SubTensionInterpreterBase
{
    private const float LidarEventCooldownSeconds = 3f;
    private float _lastLidarEventTime = float.NegativeInfinity;

    public OnScanEnergyDepletedInterpreter(UnityEngine.Object source) : base(source)
    {
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<LidarScanEnergyDepletedRawEvent>(OnLidarEnergyDepleted));
        subscriptions.Add(hub.Subscribe<SonarScanEnergyDepletedRawEvent>(OnSonarEnergyDepleted));
    }

    private void OnLidarEnergyDepleted(LidarScanEnergyDepletedRawEvent data)
    {
        if (data.Context.Time - _lastLidarEventTime < LidarEventCooldownSeconds)
        {
            return;
        }

        _lastLidarEventTime = data.Context.Time;
        publisher.TryPublish(
            context => new OnTensionChangedEvent(
                context,
                TensionReasons.LidarEnergyDepleted,
                ETensionChannel.BaseTension,
                TensionDeltaConstants.LidarEnergyDepletedBase));
    }

    private void OnSonarEnergyDepleted(SonarScanEnergyDepletedRawEvent data)
    {
        publisher.TryPublish(
            context => new OnTensionChangedEvent(
                context,
                TensionReasons.SonarEnergyDepleted,
                ETensionChannel.BaseTension,
                TensionDeltaConstants.SonarEnergyDepletedBase));
    }

    protected override void Reset()
    {
        _lastLidarEventTime = float.NegativeInfinity;
    }
}
