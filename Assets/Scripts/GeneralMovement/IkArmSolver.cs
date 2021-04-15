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
        //IkFootSolver.PositionUpdated += (sender, newPosition) => { FootPositionWasUpdated(newPosition); };
    }

    void Update()
    {
        CurrentPosition = CharacterMovement.transform.position + (IkFootSolver.OtherFoot.CurrentPosition - CharacterMovement.transform.position);
        CurrentPosition += CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
        CurrentPosition += CharacterMovement.transform.forward * ArmForward;
        CurrentPosition += CharacterMovement.transform.up * ArmHeight;
        transform.position = CurrentPosition;

        /*
        if (lerp < 1) //this has to be changed, the arm should move relative to the body, not to the world. It should move with the body, not be anchored to the ground
        {
            Vector3 armPosition = Vector3.Lerp(OldPosition, NewPosition, lerp);

            CurrentPosition = armPosition;
            lerp += Time.deltaTime * ArmSwingSpeed;
        }
        else
        {
            OldPosition = NewPosition;
        }
        */
    }

    private void FootPositionWasUpdated(Vector3 newPosition)
    {
        lerp = 0;
        OldPosition = CurrentPosition;
        NewPosition = CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
        //NewPosition += (CharacterMovement.transform.position - newPosition);
        NewPosition += CharacterMovement.transform.position - IkFootSolver.NewPosition;
        NewPosition += CharacterMovement.transform.forward * ArmForward;
        NewPosition += CharacterMovement.transform.up * ArmHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentPosition, 0.05f);
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(NewPosition, 0.05f);
    }
}
