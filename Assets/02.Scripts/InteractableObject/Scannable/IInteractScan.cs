using System;

public interface IInteractScan
{
    bool IsProgressComplete {get;}  //현재 스캔이 완료되었는가
    float CurrentProgress{get;}     //현재 스캔의 진행도
    float ProgressRatio { get; }    //현재 스캔의 진행율
    
    event Action<float> OnScanProgressChanged;
    event Action OnScanComplete;
}