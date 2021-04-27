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
    private bool punching = false;
    private Vector3 punchStartPosition;
    private Vector3 punchEndPosition;

    void Start()
    {
        CharacterMovement.OnAttack += (sender, punchPosition, attackSide) => { Punch(punchPosition, attackSide); };
    }

    void Update()
    {
        if (!punching)
        {
            CurrentPosition = CharacterMovement.transform.position + (IkFootSolver.OtherFoot.CurrentPosition - CharacterMovement.transform.position);
            CurrentPosition += CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
            CurrentPosition += CharacterMovement.transform.forward * ArmForward;
            CurrentPosition += CharacterMovement.transform.up * ArmHeight;
            transform.position = CurrentPosition;
        }
        else
        {
            if (lerp < 2)
            {
                if (lerp < 1)
                    CurrentPosition = Vector3.Lerp(punchStartPosition, punchEndPosition, lerp);
                transform.position = CurrentPosition;

                lerp += Time.deltaTime * ArmSwingSpeed;
            }
            else
            {
                punching = false;
            }
        }
    }

    private void Punch(Vector3 position, MovingCharacter.AttackSide attackSide)
    {
        if (RightArm && attackSide == MovingCharacter.AttackSide.Left)
            return;
        if (!RightArm && attackSide == MovingCharacter.AttackSide.Right)
            return;

        lerp = 0;
        punchStartPosition = CurrentPosition;
        punchEndPosition = position;
        punching = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentPosition, 0.05f);
    }
}
