//현재 타겟이 없는 라이더스캔 중을 감지하는 판독기
public class OnScanNoTargetInterpreter : SubTensionInterpreterBase
{
    private readonly float tickInterval;

    private bool isScanning;
    private bool hasTarget;
    private float elapsedTime;

    public OnScanNoTargetInterpreter(UnityEngine.Object source, float tickInterval = 1.0f) : base(source)
    {
        this.tickInterval = tickInterval;
    }

    protected override void Subscribe(GameEventHub hub)
    {
        subscriptions.Add(hub.Subscribe<LidarScanStartedRawEvent>(OnScanStarted));
        subscriptions.Add(hub.Subscribe<LidarScanStoppedRawEvent>(OnScanStopped));
        subscriptions.Add(hub.Subscribe<LidarScanTargetChangedRawEvent>(OnTargetChanged));
    }

    public override void Tick(float deltaTime)
    {
        if (isScanning == false)
        {
            return;
        }

        if (hasTarget == true)
        {
            elapsedTime = 0.0f;
            return;
        }

        elapsedTime += deltaTime;

        while (elapsedTime >= tickInterval)
        {
            elapsedTime -= tickInterval;

            publisher.TryPublish(
                context => new OnTensionIncreaseEvent(
                    context,
                    TensionReasons.LidarScanningWithNoTarget));
        }
    }

    protected override void Reset()
    {
        isScanning = false;
        hasTarget = false;
        elapsedTime = 0.0f;
    }

    private void OnScanStarted(LidarScanStartedRawEvent scanStartedEvent)
    {
        isScanning = true;
        elapsedTime = 0.0f;
    }

    private void OnScanStopped(LidarScanStoppedRawEvent scanStoppedEvent)
    {
        Reset();
    }

    private void OnTargetChanged(LidarScanTargetChangedRawEvent targetChangedEvent)
    {
        hasTarget = targetChangedEvent.CurrentTarget != null;

        if (hasTarget == true)
        {
            elapsedTime = 0.0f;
        }
    }
}