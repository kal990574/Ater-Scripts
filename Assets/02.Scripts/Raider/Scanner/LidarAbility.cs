using UnityEngine;

public class LidarAbility : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] protected LidarController _controller;

    protected virtual void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponentInParent<LidarController>();
        }
    }
}
