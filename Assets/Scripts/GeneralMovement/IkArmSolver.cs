using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkArmSolver : MonoBehaviour
{
    public IkFootSolver IkFootSolver;
    public MovingCharacter CharacterMovement;

    public bool RightArm = false;
    public float ArmDistanceToBody = 0.5f;
    public float ArmHeight = 1.5f;
    public float ArmSwingSpeed = 1.2f;
    public float ArmForward = 1.2f;

    public Vector3 CurrentPosition { get; private set; }
    public Vector3 NewPosition { get; private set; }
    public Vector3 OldPosition { get; private set; }

    private float lerp = 1;

    void Start()
    {

    }

    void Update()
    {
        CurrentPosition = CharacterMovement.transform.position + (IkFootSolver.OtherFoot.CurrentPosition - CharacterMovement.transform.position);
        CurrentPosition += CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
        CurrentPosition += CharacterMovement.transform.forward * ArmForward;
        CurrentPosition += CharacterMovement.transform.up * ArmHeight;
        transform.position = CurrentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentPosition, 0.05f);
    }
}
