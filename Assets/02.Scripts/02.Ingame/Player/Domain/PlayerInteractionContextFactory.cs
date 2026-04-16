using UnityEngine;

namespace _02.Scripts.Player
{
    public class PlayerInteractionContextFactory
    {
        private readonly PlayerController _playerController;
        private readonly PlayerHandAbility _handAbility;

        public PlayerInteractionContextFactory(
            PlayerController playerController,
            PlayerHandAbility handAbility)
        {
            _playerController = playerController;
            _handAbility = handAbility;
        }

        public InteractionContext Create(GameObject targetObject)
        {
            UsableObject usableObject = targetObject != null
                ? targetObject.GetComponent<UsableObject>()
                : null;

            return Create(usableObject, targetObject);
        }

        public InteractionContext Create(UsableObject target)
        {
            GameObject targetObject = target != null ? target.gameObject : null;
            return Create(target, targetObject);
        }

        private InteractionContext Create(UsableObject target, GameObject targetObject)
        {
            InventoryManager inventory = InventoryManager.Instance;
            string handItemInstanceId = _handAbility != null
                ? _handAbility.CurrentHandItemInstanceId
                : null;

            RuntimeItemData hand = null;
            RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
            if (runtimeInstanceManager != null && string.IsNullOrEmpty(handItemInstanceId) == false)
            {
                hand = runtimeInstanceManager.GetItemInstance(handItemInstanceId);
            }

            return new InteractionContext(
                _playerController,
                targetObject,
                target,
                inventory,
                _handAbility,
                handItemInstanceId,
                hand);
        }
    }
}
