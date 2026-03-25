using UnityEngine;

public class GettableObject : InteractableObject
{
    [SerializeField] private int _itemId;
    [SerializeField] private Vector3 _pickUpOffset;
    [SerializeField] private InventoryItemState _stateOverrides;

    public Vector3 PickUpOffset => _pickUpOffset;

    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        InventoryItemInstance itemInstance = InventoryManager.Instance.CreateItemInstance(_itemId);
        if (itemInstance == null)
        {
            return;
        }

        itemInstance.State.ApplyOverrides(_stateOverrides);
        InventoryManager.Instance.TryAddItem(itemInstance);
        OnInteractActivate();
        gameObject.SetActive(false);
    }
}
