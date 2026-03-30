//게임 이벤트의 공통 규약, 이벤트가 공통 메타데이터를 가지게 한다.
public interface IGameEvent
{
    GameEventContext Context { get; }
}
