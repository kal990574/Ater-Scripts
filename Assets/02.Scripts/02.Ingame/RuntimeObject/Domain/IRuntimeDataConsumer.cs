public interface IRuntimeDataConsumer
{
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }
    void SetRuntimeData(IRuntimeView runtimeView);
}
