public interface IScannableQTEHandler
{
    EScanState State { get; }

    void PauseScanning();
    void ResumeScanningAfterQte();
    void HandleQteFailure(float failPenalty);
    bool HandleQteSuccess();
    bool HandleQteGreatSuccess(float greatSuccessBonus);
}
