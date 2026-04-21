using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerInteractAbility : PlayerAbility
    {
        public void Interact(IDetectable target)
        {
            if (target == null)
            {
                Debug.Log("[PlayerInteractAbility] : target is null.");
                return;
            }

            if (!target.Transform.TryGetComponent(out IRuntimeInteractObject interactableObject))
            {
                Debug.Log("[PlayerInteractAbility] : target does not have an interactable runtime object.");
                return;
            }

            if (target.Transform.TryGetComponent(out Interactable interactable) && !interactable.IsInteractActive)
            {
                return;
            }

            InteractionContext context = _owner != null
                ? _owner.CreateInteractionContext(target.Transform.gameObject)
                : InteractionContext.CreateEmpty(target.Transform.gameObject);
            interactableObject.Interact(context);
        }
    }
}
