using UnityEngine;

namespace Warwlock.PlayerController
{
    public interface IMovement
    {
        bool IsGrounded { get; }
        bool IsJumped { get; }
        Vector3 MovementDirectionSpeed { get; }
        void SetMovementEnabledSimulate(bool isEnabled);
    }
}