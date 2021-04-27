using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingCharacter : MonoBehaviour
{
    public enum AttackSide
    {
        Right, Left, Both
    }

    public abstract bool IsGrounded { get; }
    public abstract bool StandingStill { get; }

    public delegate void AttackHandler(object sender, Vector3 attackPoint, AttackSide attackSide);
    public abstract event AttackHandler OnAttack;

    public abstract void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength);
}
