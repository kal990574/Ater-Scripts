using UnityEngine;

//해당 아이템을 들고, 떨어뜨리고, 던지고, 획득할수 있음
public interface ICarryable
{
    void OnPickUp();  //단축키로 들기
    void OnDrop();    //들고있는 아이템 떨어뜨리기
    void OnThrow();   //들고있는 아이템 던지기
    void OnGet();     //아이템을 인벤토리에 넣기
}
