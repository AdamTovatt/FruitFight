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
    public Vector3 Offset;

    public bool IsMoving { get { return lerp < 1; } }

    private Transform character;
    private float appliedFootSpacing;

    public Vector3 GroundForward { get { return groundForward; } }
    public Vector3 OldPosition { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    public GroundedPositionInformation NewPosition { get; private set; }

    private float lerp = 1;

    private bool inDefaultPosition = true;

    private AverageVelocityKeeper characterVelocity;
    private GroundedChecker groundedChecker;

    public delegate void PositionUpdatedEventHandler(Vector3 newPosition);
    public event PositionUpdatedEventHandler PositionUpdated;

    private GroundedPositionInformation currentGroundedPosition;
    private MoveOnTrigger parent;
    private Vector3 groundForward;

    void Start()
    {
        characterVelocity = CharacterMovement.gameObject.GetComponent<AverageVelocityKeeper>();
        groundedChecker = CharacterMovement.gameObject.GetComponent<GroundedChecker>();
        character = CharacterMovement.transform;
        appliedFootSpacing = FootSpacing * (RightFoot ? 1f : -1f);

        if (CharacterMovement.GetType() == typeof(PlayerMovement))
        {
            ((PlayerMovement)CharacterMovement).OnParentUpdated += ParentUpdated;
            ((PlayerMovement)CharacterMovement).OnLandedOnBouncyObject += LandedOnBouncyObject;
        }

        CharacterMovement.RegisterFoot(this);

        CurrentPosition = GetGroundPosition(0).Position;
        lerp = 1;
    }

    private void OnDestroy()
    {
        if (CharacterMovement.GetType() == typeof(PlayerMovement))
        {
            ((PlayerMovement)CharacterMovement).OnParentUpdated -= ParentUpdated;
            ((PlayerMovement)CharacterMovement).OnLandedOnBouncyObject -= LandedOnBouncyObject;
        }

    }

    void Update()
    {
        transform.position = CurrentPosition;// += (parent == null ? Vector3.zero : -parent.CurrentMovement);

        float appliedStepDistance = Mathf.Max(characterVelocity.Velocity * StepDistance * 0.3f, MinStepDistance);

        GroundedPositionInformation searchPosition = GetGroundPosition(appliedStepDistance);

        bool isInAir = false;
        if (groundedChecker == null ? CharacterMovement.IsGrounded : groundedChecker.IsGrounded)
        {
            if (((CharacterMovement.StandingStill == null) || (CharacterMovement.StandingStill != null && !((bool)CharacterMovement.StandingStill))) && characterVelocity.Velocity > 0.01f) //character is moving
            {
                float distance = Vector3.Distance(NewPosition.AppliedPosition, searchPosition.AppliedPosition);
                if (distance > appliedStepDistance && ((!OtherFoot.IsMoving && lerp >= 1) || distance > StepDistance * 1.8f))
                {
                    inDefaultPosition = false;
                    lerp = 0;
                    OldPosition = CurrentPosition;
                    NewPosition = searchPosition;
                    currentGroundedPosition = searchPosition;
                    PositionUpdated?.Invoke(NewPosition.AppliedPosition);
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
        else //in air
        {
            isInAir = true;
            inDefaultPosition = false;
            CurrentPosition = character.transform.position + (Vector3.up * FootJumpPositionHeight) + (character.right * appliedFootSpacing);
            currentGroundedPosition = new GroundedPositionInformation() { Position = CurrentPosition };
        }

        if (lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(OldPosition, currentGroundedPosition.AppliedPosition, lerp);

            if (!inDefaultPosition)
                footPosition.y += Mathf.Sin(lerp * Mathf.PI) * StepHeight;

            CurrentPosition = footPosition;
            lerp += Time.deltaTime * StepSpeed;
        }
        else
        {
            if (currentGroundedPosition == null)
            {
                currentGroundedPosition = GetGroundPosition(0);
            }

            OldPosition = currentGroundedPosition.AppliedPosition;

            if (!isInAir && NewPosition != null)
                CurrentPosition = NewPosition.AppliedPosition;

            if (!CharacterMovement.StopFootSetDefault)
                inDefaultPosition = false;
        }
    }

    private void LandedOnBouncyObject()
    {
        SetDefaultPosition();
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
            currentGroundedPosition = GetGroundPosition(0);
            NewPosition = currentGroundedPosition;
        }
    }

    private GroundedPositionInformation GetGroundPosition(float forward)
    {
        Ray ray = new Ray(character.position + (Vector3.up * 0.5f) + (character.right * appliedFootSpacing) + (character.forward * forward), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10))
        {
            groundForward = Quaternion.AngleAxis(-90, transform.right) * info.normal;
            GroundedPositionInformation result = new GroundedPositionInformation() { Transform = info.transform, RelativePosition = (info.point + Offset) - info.transform.position };
            return result;
        }

        Debug.LogWarning("No ground position found for next footstep");
        return currentGroundedPosition == null ? new GroundedPositionInformation() { Position = character.transform.position + (Vector3.up * FootJumpPositionHeight) + (character.right * appliedFootSpacing) } : currentGroundedPosition;
    }

    private void OnDrawGizmos()
    {
        if (NewPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(NewPosition.AppliedPosition, 0.05f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, groundForward);
    }
}
