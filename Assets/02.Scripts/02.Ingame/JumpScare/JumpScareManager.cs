using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    private readonly CompositeSubscription _subscriptions = new CompositeSubscription();

    private void Start()
    {
        GameEventHub hub = GameEventHub.Instance;

        if (hub == null)
        {
            Debug.LogWarning("[MainJumpscareManager] GameEventHub가 존재하지 않습니다.");
            return;
        }

        _subscriptions.Add(hub.Subscribe<ItemGetEvent>(OnGetItemKey));
    }

    private void OnDestroy()
    {
        _subscriptions.Dispose();
    }

    private void OnGetItemKey(ItemGetEvent gameEvent)
    {
        if (gameEvent.ItemId == 3)
        {
            Debug.Log($"{gameEvent.InstanceId} 인스턴스 획득");
            Debug.Log("메인 점프스케어 요청: KeyGet_MannequinLook");
        }
    }
}