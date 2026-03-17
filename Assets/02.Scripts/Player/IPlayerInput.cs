using UnityEngine;

namespace _02.Scripts.Player
{
    public interface IPlayerInput
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool LmbInput { get; }
    }
}