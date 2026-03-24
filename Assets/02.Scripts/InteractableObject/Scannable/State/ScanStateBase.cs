public abstract class ScanStateBase : IScanState
{
    protected readonly InteractScanObject Owner;

    public abstract EScannableState StateType { get; }

    protected ScanStateBase(InteractScanObject owner)
    {
        Owner = owner;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void OnScanning(float deltaTime)
    {
    }

    public virtual void OnScanLost()
    {
    }
}
