public interface INeedRuntimeData
{
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }
    void SetRuntimeData(IRuntimeView runtimeView);
}
