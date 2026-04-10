using UnityEngine;

public readonly struct QteRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public GameObject Invoker { get; }
    public EQTEType QTEType { get; }
    public EQuickTimeEventResult Result { get; }

    public QteRawEvent(
        GameEventContext context,
        GameObject invoker,
        EQTEType qteType,
        EQuickTimeEventResult result)
    {
        Context = context;
        Invoker = invoker;
        QTEType = qteType;
        Result = result;
    }
}