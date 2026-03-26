using _02.Scripts.Player;
using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    public void Interact(IDetectableObject target)
    {
        if (target == null)
        {
            Debug.Log("[PlayerInteractAbility] : 지정된 대상이 없음");
            return;
        }
        
        if (!TryGetComponent(out IInteractObject interactableObject))
        {
            Debug.Log("[PlayerInteractAbility] :해당 대상은 상호작용 가능하지 않음");
            return;
        }

        interactableObject.Interact();
    }
}
