public interface IStateApplier
{
    IRuntimeView RuntimeView { get; }
    void ApplyState(RuntimeView binder);
}