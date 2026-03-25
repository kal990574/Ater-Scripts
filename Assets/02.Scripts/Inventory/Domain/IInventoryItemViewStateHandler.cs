//인스턴스의 State를 적용
//ex) 박스가 열려있는지(모델변경 몇 열쇠 아이템 활성화), 열쇄를 획득했는지(열쇄 제거)
public interface IInventoryItemViewStateHandler
{
    void ApplyState(InventoryItemViewContext context);
}
