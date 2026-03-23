public interface IScannable
{
    bool IsProgressComplete {get;}  //현재 스캔이 완료되었는가
    float CurrentProgress{get;}     //현재 스캔의 진행도
    float ProgressRatio { get; }    //현재 스캔의 진행율

    void OnScanStarted();       //스캔 시작
    void OnScanning(float deltaTime);          //스캔중
    void OnScanStopped();       //스캔 종료
    void OnScanCompleted();     //스캔 완료

}