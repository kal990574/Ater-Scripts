public interface IItemInstance
{
    string InstanceId { get; }
    ItemInstanceData ItemInstanceData { get; }
    InventoryManager InventoryManager { get; }
    void Bind(string instanceId, InventoryManager inventoryManager);
    void RefreshView();
}
