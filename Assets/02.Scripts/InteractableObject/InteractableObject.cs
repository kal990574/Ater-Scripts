using UnityEngine;

//상호작용 가능한 오브젝트의 최상위 관리자
//각종 효과를 관리함
public class InteractableObject : MonoBehaviour
{
    private IScannable _scannable;
    private IHoverable _hoverable;
    private ICarryable _carryable;
    private IUseable _useable;
}
