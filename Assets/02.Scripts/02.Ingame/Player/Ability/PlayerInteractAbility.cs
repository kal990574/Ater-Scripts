using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerInteractAbility : PlayerAbility
    {
        public void Interact(IDetectable target)
        {
            if (target == null)
            {
                Debug.Log("[PlayerInteractAbility] : 吏?뺣맂 ??곸씠 ?놁쓬");
                return;
            }

            if (!target.Transform.TryGetComponent(out IRuntimeInteractObject interactableObject))
            {
                Debug.Log("[PlayerInteractAbility] :?대떦 ??곸? ?곹샇?묒슜 媛?ν븯吏 ?딆쓬");
                return;
            }

            InteractionContext context = _owner != null
                ? _owner.CreateInteractionContext(target.Transform.gameObject)
                : InteractionContext.CreateEmpty(target.Transform.gameObject);
            interactableObject.Interact(context);
        }
    }
}
