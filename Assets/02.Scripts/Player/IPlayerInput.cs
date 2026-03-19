using UnityEngine;

namespace _02.Scripts.Player
{
    public interface IPlayerInput
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool LmbPressInput { get; }
        bool LmbReleaseInput { get; }
        bool RmbPressInput { get; }
        bool RmbReleaseInput { get; }
        bool InteractInput { get; }
    }
}