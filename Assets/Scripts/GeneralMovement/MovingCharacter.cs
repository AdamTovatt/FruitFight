using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingCharacter : MonoBehaviour
{
    public abstract bool IsGrounded { get; }
    public abstract bool StandingStill { get; }

    public delegate void AttackHandler(object sender, Vector3 attackPoint);
    public abstract event AttackHandler OnPunched;
}
