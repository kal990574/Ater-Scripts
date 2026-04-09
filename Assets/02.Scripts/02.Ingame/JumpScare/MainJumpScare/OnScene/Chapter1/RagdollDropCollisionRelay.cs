using UnityEngine;

public class RagdollDropCollisionRelay : MonoBehaviour
{
    private RagdollDropJumpScare _owner;

    public void Initialize(RagdollDropJumpScare owner)
    {
        _owner = owner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_owner == null)
        {
            return;
        }

        _owner.NotifyRagdollCollision(collision);
    }
}
