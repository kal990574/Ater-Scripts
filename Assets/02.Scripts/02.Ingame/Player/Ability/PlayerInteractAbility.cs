using _02.Scripts.Player;
using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    public void Interact(IDetectable target)
    {
        if (target == null)
        {
            Debug.Log("[PlayerInteractAbility] : 지정된 대상이 없음");
            return;
        }
        
        if (!target.Transform.TryGetComponent(out IRuntimeInteractObject interactableObject))
        {
            Debug.Log("[PlayerInteractAbility] :해당 대상은 상호작용 가능하지 않음");
            return;
        }

        InteractionContext context = InteractionContext.For(_owner.gameObject, target.Transform.gameObject);
        interactableObject.Interact(context);
    }
}
