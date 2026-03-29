public interface INeedItemInstance
{
    RuntimeItemData RuntimeItemData { get; }
    void SetInstance(IRuntimeView runtimeView);
}
