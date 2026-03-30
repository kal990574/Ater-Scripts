public interface IRuntimeView
{
    string InstanceId { get; }
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }
    InventoryManager InventoryManager { get; }
    void Bind(string instanceId, InventoryManager inventoryManager);
    void RefreshView();
}
