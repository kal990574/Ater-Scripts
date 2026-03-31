using UnityEngine;

//텐션 관리
//텐션을 통해 점프스케어 매니저를 호출해 서브 점프스케어 발생
public class TensionManager : MonoBehaviour
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

        _subscriptions.Add(hub.Subscribe<GetInteractEvent>(OnGetInteract));
        _subscriptions.Add(hub.Subscribe<ReleaseInteractEvent>(OnReleaseInteract));
        _subscriptions.Add(hub.Subscribe<ScanCompleteEvent>(OnScanComplete));
        _subscriptions.Add(hub.Subscribe<UseInteractEvent>(OnUseInteract));
        _subscriptions.Add(hub.Subscribe<PuzzleFailEvent>(OnPuzzleFailed));
        _subscriptions.Add(hub.Subscribe<PuzzleSuccessEvent>(OnPuzzleSuccess));
        _subscriptions.Add(hub.Subscribe<QTEFailEvent>(OnQteFailed));
        _subscriptions.Add(hub.Subscribe<QTEGoodEvent>(OnQteGood));
        _subscriptions.Add(hub.Subscribe<QTEGreatEvent>(OnQteGreat));
    }
    
    private void OnDestroy()
    {
        _subscriptions.Dispose();
    }
    
    private void OnGetInteract(GetInteractEvent eventData)
    {
        if (eventData.ItemId == 3)
        {
            Debug.Log($"{eventData.Context.SourceName} 획득");
        }
    }

    private void OnReleaseInteract(ReleaseInteractEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} 던짐");
    }

    private void OnScanComplete(ScanCompleteEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} 스캔 완료");
    }

    private void OnUseInteract(UseInteractEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} 사용");
    }
    
    private void OnPuzzleSuccess(PuzzleSuccessEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} 성공");
    }
    
    private void OnPuzzleFailed(PuzzleFailEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} 실패");
    }
    
    private void OnQteFailed(QTEFailEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} QTE 실패");
    }

    private void OnQteGood(QTEGoodEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} QTE 성공");
    }

    private void OnQteGreat(QTEGreatEvent eventData)
    {
        Debug.Log($"{eventData.Context.SourceName} QTE 대성공");
    }
}
