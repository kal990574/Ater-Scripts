//게임 이벤트 버스의 코어 인터페이스
using System;

public interface IGameEventBus
{
    IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGameEvent;   //구독
    void Publish<T>(in T gameEvent) where T : struct, IGameEvent;               //발행
    void Clear();                                                               //모두 제거
}