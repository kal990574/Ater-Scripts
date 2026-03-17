public enum ELidarObjectState
{
    Default,        //스캔중이지 않으며 진행도가 0인상태
    OnProgress,     //현재 스캔중인 상태 진행도 증가
    OnReturn,       //도중에 스캔이 끊긴 상태이며 진행도가 점점 감소
    OnMinigame,     //미니게임 중인 상태
    OnCompleted     //스캔이 완료된 상태
}