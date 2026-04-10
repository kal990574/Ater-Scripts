//라이더 스캔에서의 로우  이벤트

using Unity.Collections;/// <summary>
/// 라이더 스캔 시작
/// </summary>
public readonly struct LidarScanStartedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public LidarScanStartedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

/// <summary>
/// 라이더 스캔 종료 
/// </summary>
public readonly struct LidarScanStoppedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public LidarScanStoppedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

/// <summary>
/// 라이더 스캔 대상 변경
/// </summary>
public readonly struct LidarScanTargetChangedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public ScannableObject PreviousTarget { get; }
    public ScannableObject CurrentTarget { get; }

    public LidarScanTargetChangedRawEvent(
        GameEventContext context,
        ScannableObject previousTarget,
        ScannableObject currentTarget)
    {
        Context = context;
        PreviousTarget = previousTarget;
        CurrentTarget = currentTarget;
    }
}

/// <summary>
/// 라이더 스캔 대상 스캔 완료 이벤트
/// </summary>
public readonly struct LidarScanTargetCompletedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public ScannableObject Target { get; }

    public LidarScanTargetCompletedRawEvent(
        GameEventContext context,
        ScannableObject target)
    {
        Context = context;
        Target = target;
    }
}

public readonly struct LidarScanEnergyDepletedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public LidarScanEnergyDepletedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}
