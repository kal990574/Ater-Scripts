using UnityEngine;

//아이템을 타게팅 할 수 있음
public interface IHoverable
{
    void OnHoverEnter();        //호버 상태에 진입한 경우(타겟팅)
    void OnHoverExit();         //호버 상태에서 빠져나온경우(언타겟팅)
}