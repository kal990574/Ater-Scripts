namespace _02.Scripts._02.Ingame.Tutorial.Domain
{
    public enum TutorialStepId
    {
        None = 0,
        Movement = 1,            // WASD
        Sonar = 2,               // 소나 스캔
        GoThroughDoor = 3,       // 문 통과
        LidarBlackboard = 4,     // 칠판 라이더 스캔 완료
        InteractBlackboard = 5,  // 칠판 E키 상호작용
        FindKey = 6,             // 열쇠 획득
        EquipKey = 7,            // 숫자키 장착
        ScannerToggle = 8,       // Q키 스캐너 토글
        FindNote = 9,            // 쪽지 획득
        OpenInventory = 10,      // TAB 인벤토리
        InspectNote = 11,        // 아이템 선택 검사
        ZoomItem = 12,           // 스크롤 줌
        RotateItem = 13,         // 드래그 회전
        OpenPadlock = 14,        // 자물쇠 열기
        OpenDoor = 15,           // 문 열기
        LidarOnDoor = 16,        // 문 스캔하기
        UseKeyOnDoor = 17,       // 열쇠로 문 열기
    }
}