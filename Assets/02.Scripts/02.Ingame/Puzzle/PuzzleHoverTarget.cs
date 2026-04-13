using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleHoverTarget : MonoBehaviour
{
    [SerializeField] private string _hoverText;

    private GameEventPublisher _publisher;

    private void Awake()
    {
        _publisher = new GameEventPublisher();
        _publisher.SetSource(this);
    }

    private void Start()
    {
        _publisher.TryPublish(ctx => new InteractPromptRawEvent(ctx, true, _hoverText));
    }

}
