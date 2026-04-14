using System;
using UnityEngine;

// 게임 이벤트를 발행하는 스크립트가 보유하여 사용하는 헬퍼 클래스
public class GameEventPublisher
{
    private UnityEngine.Object sourceObject;

    public void SetSource(UnityEngine.Object source)
    {
        sourceObject = source;
    }

    public bool TryPublish<T>(Func<GameEventContext, T> factory, string defaultName = "Unknown") where T : struct, IGameEvent
    {
        if (factory == null)
        {
            return false;
        }

        GameEventHub hub = GameEventHub.Instance;
        if (hub == null)
        {
            return false;
        }

        string sourceName = sourceObject != null ? sourceObject.name : defaultName;
        int sourceId = sourceObject != null ? sourceObject.GetInstanceID() : 0;

        GameEventContext context = new GameEventContext(
            Time.frameCount,
            Time.time,
            hub.NextSequence(),
            sourceId,
            sourceName);

        hub.Publish(factory(context));
        return true;
    }
}