public interface IItemInstance
{
    string InstanceId { get; }
    InstanceData InstanceData { get; }
    InventoryManager InventoryManager { get; }
    void Bind(string instanceId, InventoryManager inventoryManager);
    void RefreshView();
}
