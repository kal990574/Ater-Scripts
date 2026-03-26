public interface IItemInstance
{
    ItemInstance ItemInstance { get; }
    InventoryManager InventoryManager { get; }
    void Bind(ItemInstance itemInstance, InventoryManager inventoryManager);
    void RefreshView();
}