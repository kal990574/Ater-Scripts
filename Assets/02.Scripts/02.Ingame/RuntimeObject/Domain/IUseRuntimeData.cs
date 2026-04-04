public interface IUseRuntimeData
{
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }
    void SetRuntimeData(IRuntimeView runtimeView);
}
