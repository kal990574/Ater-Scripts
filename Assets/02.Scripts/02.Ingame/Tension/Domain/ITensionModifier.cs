//todo : TensionManager에 부착
public interface ITensionModifier
{
    void AddTension(float amount, string reason);
    void DecreaseTension(float amount, string reason);
}