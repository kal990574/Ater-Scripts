public interface ILidarTargetState
{
    ELidarTargetState StateType { get; }

    void Enter();
    void Exit();
    void Tick(float deltaTime);
    void OnScanning(float deltaTime);
    void OnScanLost();
}