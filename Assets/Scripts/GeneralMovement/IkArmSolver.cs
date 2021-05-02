using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkArmSolver : MonoBehaviour
{
    public IkFootSolver IkFootSolver;
    public MovingCharacter CharacterMovement;
    public TrailRenderer Trail;

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
    private Vector3 characterPositionAtStartPunch;

    void Start()
    {
        CharacterMovement.OnAttack += (sender, punchPosition, attackSide) => { Punch(punchPosition, attackSide); };
        Trail.enabled = false;
    }

    void Update()
    {
        Vector3 swingPosition = CharacterMovement.transform.position + (IkFootSolver.OtherFoot.CurrentPosition - CharacterMovement.transform.position);
        swingPosition += CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
        swingPosition += CharacterMovement.transform.forward * ArmForward;
        swingPosition += CharacterMovement.transform.up * ArmHeight;

        if (!punching)
        {
            CurrentPosition = swingPosition;
            transform.position = CurrentPosition;
        }
        else
        {
            Vector3 characterOffsetSincePunchStart = CharacterMovement.transform.position - characterPositionAtStartPunch;
            punchEndPosition += characterOffsetSincePunchStart;

            if (lerp < 3)
            {
                if (lerp < 1)
                    CurrentPosition = Vector3.Lerp(punchStartPosition, punchEndPosition, lerp);
                else if (lerp > 1 && lerp < 2)
                    CurrentPosition = punchEndPosition;
                else
                    CurrentPosition = Vector3.Lerp(punchEndPosition, swingPosition, lerp - 2);

                transform.position = CurrentPosition;

                lerp += Time.deltaTime * ArmSwingSpeed;
            }
            else
            {
                Trail.enabled = false;
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
        characterPositionAtStartPunch = CharacterMovement.transform.position;
        Trail.enabled = true;
        punching = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentPosition, 0.05f);
    }
}
