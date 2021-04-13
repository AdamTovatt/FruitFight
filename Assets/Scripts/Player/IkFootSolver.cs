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

    private Vector3 oldPosition;
    private Vector3 currentPosition;
    private Vector3 newPosition;

    private float lerp;

    private bool inDefaultPosition = true;
    private bool inJumpPosition = false;

    AverageVelocityKeeper characterVelocity;

    void Start()
    {
        characterVelocity = CharacterMovement.gameObject.GetComponent<AverageVelocityKeeper>();
        character = CharacterMovement.transform;
        appliedFootSpacing = FootSpacing * (RightFoot ? 1f : -1f);

        currentPosition = GetGroundPosition(0);
        lerp = 1;
    }

    void Update()
    {
        transform.position = currentPosition;

        float appliedStepDistance = Mathf.Max(characterVelocity.Velocity * StepDistance * 0.3f, MinStepDistance);

        Vector3 searchPosition = GetGroundPosition(appliedStepDistance);

        if (CharacterMovement.IsGrounded)
        {
            inJumpPosition = false;
            if (!CharacterMovement.StandingStill && characterVelocity.Velocity != 0)
            {
                float distance = Vector3.Distance(newPosition, searchPosition);
                if (distance > appliedStepDistance && ((!OtherFoot.IsMoving && lerp >= 1) || distance > StepDistance * 1.8f))
                {
                    inDefaultPosition = false;
                    lerp = 0;
                    oldPosition = currentPosition;
                    newPosition = searchPosition;
                }
            }
            else
            {
                if (!inDefaultPosition)
                {
                    inDefaultPosition = true;
                    searchPosition = GetGroundPosition(0);
                    lerp = 0;
                    oldPosition = currentPosition;
                    newPosition = searchPosition;
                }
            }
        }
        else
        {
            currentPosition = character.transform.position + (Vector3.up * FootJumpPositionHeight) + (character.right * appliedFootSpacing);
        }

        if (lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);

            if (!inDefaultPosition)
                footPosition.y += Mathf.Sin(lerp * Mathf.PI) * StepHeight;

            currentPosition = footPosition;
            lerp += Time.deltaTime * StepSpeed;
        }
        else
        {
            oldPosition = newPosition;
        }
    }

    private Vector3 GetGroundPosition(float forward)
    {
        Ray ray = new Ray(character.position + (Vector3.up * 0.5f) + (character.right * appliedFootSpacing) + (character.forward * forward), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10))
            return info.point;

        throw new System.Exception("No ground position found for next footstep");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.05f);
    }
}
