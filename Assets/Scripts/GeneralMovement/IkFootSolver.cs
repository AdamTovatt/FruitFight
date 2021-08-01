using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkFootSolver : MonoBehaviour
{
    public MovingCharacter CharacterMovement;
    public float FootSpacing = 0.3f;
    public bool RightFoot = false;
    public float StepDistance = 0.5f;
    public float MinStepDistance = 0.2f;
    public float StepHeight = 0.4f;
    public float StepSpeed = 10f;
    public float FootJumpPositionHeight = 1f;
    public IkFootSolver OtherFoot;

    public bool IsMoving { get { return lerp < 1; } }

    private Transform character;
    private float appliedFootSpacing;

    public Vector3 OldPosition { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    public Vector3 NewPosition { get; private set; }

    private float lerp;

    private bool inDefaultPosition = true;

    AverageVelocityKeeper characterVelocity;

    public delegate void PositionUpdatedEventHandler(object sender, Vector3 newPosition);
    public event PositionUpdatedEventHandler PositionUpdated;

    private MoveOnTrigger parent;

    void Start()
    {
        characterVelocity = CharacterMovement.gameObject.GetComponent<AverageVelocityKeeper>();
        character = CharacterMovement.transform;
        appliedFootSpacing = FootSpacing * (RightFoot ? 1f : -1f);

        if (CharacterMovement.GetType() == typeof(PlayerMovement))
        {
            ((PlayerMovement)CharacterMovement).OnParentUpdated += ParentUpdated;
        }

        CurrentPosition = GetGroundPosition(0);
        lerp = 1;
    }

    void Update()
    {
        Debug.Log(parent?.CurrentMovement);
        transform.position = CurrentPosition += (parent == null ? Vector3.zero : -parent.CurrentMovement);

        float appliedStepDistance = Mathf.Max(characterVelocity.Velocity * StepDistance * 0.3f, MinStepDistance);

        Vector3 searchPosition = GetGroundPosition(appliedStepDistance);

        if (CharacterMovement.IsGrounded)
        {
            if (!CharacterMovement.StandingStill && characterVelocity.Velocity > 0) //character is moving
            {
                float distance = Vector3.Distance(NewPosition, searchPosition);
                if (distance > appliedStepDistance && ((!OtherFoot.IsMoving && lerp >= 1) || distance > StepDistance * 1.8f))
                {
                    inDefaultPosition = false;
                    lerp = 0;
                    OldPosition = CurrentPosition;
                    NewPosition = searchPosition;
                    PositionUpdated?.Invoke(this, NewPosition);
                }
            }
            else //character is not moving
            {
                if (!inDefaultPosition)
                {
                    SetDefaultPosition();
                }
            }
        }
        else
        {
            inDefaultPosition = false;
            CurrentPosition = character.transform.position + (Vector3.up * FootJumpPositionHeight) + (character.right * appliedFootSpacing);
        }

        if (lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(OldPosition, NewPosition, lerp);

            if (!inDefaultPosition)
                footPosition.y += Mathf.Sin(lerp * Mathf.PI) * StepHeight;

            CurrentPosition = footPosition;
            lerp += Time.deltaTime * StepSpeed;
        }
        else
        {
            OldPosition = NewPosition;
            if (!CharacterMovement.StopFootSetDefault)
                inDefaultPosition = false;
        }
    }

    private void ParentUpdated(MoveOnTrigger newParent)
    {
        parent = newParent;
    }

    public void SetDefaultPosition()
    {
        if (!inDefaultPosition)
        {
            inDefaultPosition = true;
            lerp = 0;
            OldPosition = CurrentPosition;
            NewPosition = GetGroundPosition(0);
        }
    }

    private Vector3 GetGroundPosition(float forward)
    {
        Ray ray = new Ray(character.position + (Vector3.up * 0.5f) + (character.right * appliedFootSpacing) + (character.forward * forward), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10))
            return info.point;

        Debug.LogWarning("No ground position found for next footstep");
        return CurrentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(NewPosition, 0.05f);
    }
}
