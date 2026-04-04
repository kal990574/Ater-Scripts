public interface IRuntimeView
{
    string InstanceId { get; }
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }

    void Bind(RuntimeData runtimeData);
    void RefreshView();
}