using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MovingCharacter
{
    public NavMeshAgent Navigation;

    public override bool StopFootSetDefault { get { return false; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    public override bool? StandingStill { get { return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    private GroundedChecker groundedChecker;

    private void Awake()
    {
        groundedChecker = gameObject.GetComponent<GroundedChecker>();
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        Debug.Log("robot step");
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        Debug.Log("robot attacked");
    }

}
