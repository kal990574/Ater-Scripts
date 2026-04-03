namespace _02.Scripts._02.Ingame.Tutorial.Domain
{
    public enum TutorialStepId
    {
        None = 0,
        Movement = 1,       // ① WASD
        Sonar = 2,           // ② 소나 스캔                  
        Lidar = 3,           // ③ 라이더 스캔
        Interact = 4,        // ④ E키 상호작용               
        Equip = 5,           // ⑤ 숫자키 장착 + Q 스캐너 토글
        Inventory = 6,       // ⑥ TAB 인벤토리               
        Inspect = 7,         // ⑥-a 아이템 클릭
        Manipulate = 8,      // ⑥-b 드래그/스크롤            
    }  
}