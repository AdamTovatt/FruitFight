using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingCharacter : MonoBehaviour
{
    public enum AttackSide
    {
        Right, Left, Both
    }

    public GameObject PunchSoundEffectPrefab;

    public abstract bool StopFootSetDefault { get; }
    public abstract bool IsGrounded { get; }
    public abstract bool? StandingStill { get; }

    public delegate void AttackHandler(Vector3 attackPoint, AttackSide attackSide);
    public abstract event AttackHandler OnAttack;

    public abstract void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength);
    public abstract void StepWasTaken(Vector3 stepPosition);

    public void RegisterFoot(IkFootSolver foot)
    {
        foot.PositionUpdated += StepWasTaken;
    }
}
