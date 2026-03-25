using UnityEngine;

public class InteractPoint : MonoBehaviour
{
    private IExamineInteractable _examineInteractable;
    private ExamineItemBinder _binder;

    private void Awake()
    {
        MonoBehaviour[] behaviours = GetComponentsInParent<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (_examineInteractable == null && behaviour is IExamineInteractable interactableUI)
            {
                _examineInteractable = interactableUI;
            }
            
        }

        _binder = GetComponentInParent<ExamineItemBinder>();
    }

    public void OnClick()
    {
        if (_examineInteractable != null && _binder != null)
        {
            _examineInteractable.Interact(_binder);
        }
    }
}
