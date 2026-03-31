using UnityEngine;

//게임 이벤트를 발생할수 있는 스크립트는 해당 클래스를 상속받는다.
public abstract class GameEventPublisher : MonoBehaviour
{
    protected bool TryGetHub(out GameEventHub hub)
    {
        hub = GameEventHub.Instance;
        return hub != null;
    }

    protected GameEventContext CreateContext()
    {
        GameEventHub hub = GameEventHub.Instance;

        if (hub == null)
        {
            return default;
        }

        return hub.CreateContext(this);
    }
}