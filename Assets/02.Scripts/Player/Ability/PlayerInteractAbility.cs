using _02.Scripts.Player;
using UnityEngine;

public class PlayerInteractAbility : PlayerAbility
{
    public void Interact(InteractController target)
    {
        if (target == null)
        {
            Debug.Log("상호작용 대상이 없음");
            return;
        }

        if (target.TryInteract() == false)
        {
            Debug.Log("해당 오브젝트는 사용가능한 오브젝트가 아님");
        }
    }
}
