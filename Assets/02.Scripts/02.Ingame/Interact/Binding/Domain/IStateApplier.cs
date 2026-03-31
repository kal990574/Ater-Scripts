
//변경된 바인드를 적용
public interface IStateApplier
{
    IRuntimeView RuntimeView { get; }
    void ApplyState(RuntimeView binder);
    
}
