using UnityEngine;

//해당 아이템을 들고, 떨어뜨리고, 던지고, 획득할수 있음
public interface ICarryable
{
    void PickUp();  //들기
    void Drop();    //떨어뜨리기
    void Throw();   //던지기
    void Get();     //획득하기
}
