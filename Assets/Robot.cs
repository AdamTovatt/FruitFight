using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MovingCharacter
{
    public float RoamNewTargetRange = 5f;
    public float PersonalBoundaryDistance = 0.7f;
    public NavMeshAgent Navigation;
    public UniqueSoundSource UniqueSoundSource;
    public FootStepAudioSource FootStepAudio;
    public Health Health;
    public RobotHatchController Hatches;

    public override bool StopFootSetDefault { get { return false; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    public override bool? StandingStill { get { return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    public float DistanceToTargetSquared { get { return (TargetPosition - transform.position).sqrMagnitude; } }
    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; TargetWasUpdated(); } }
    private Vector3 _targetPosition;

    private GroundedChecker groundedChecker;
    private float personalBoundaryDistanceSquared;
    private bool isFacingTarget = false;
    private float rotationLerpTime = 0;
    private float standingStillTime = 0;

    private void Awake()
    {
        groundedChecker = gameObject.GetComponent<GroundedChecker>();
        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;
    }

    private void Start()
    {
        Health.OnHealthUpdated += () => { Hatches.Open(); };
    }

    private void Update()
    {
        if (CustomNetworkManager.HasAuthority)
            Think();

        if (!isFacingTarget)
        {
            Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), rotationLerpTime);
            rotationLerpTime += Time.deltaTime * 4 * 0.01f;

            if (Quaternion.Angle(Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), transform.rotation) <= 1 || DistanceToTargetSquared <= personalBoundaryDistanceSquared)
            {
                StartedFacingTarget();
            }
        }
    }

    private void Think()
    {
        if(TargetPosition == Vector3.zero)
        {
            FindNewRoamTarget();
        }

        if (DistanceToTargetSquared < personalBoundaryDistanceSquared)
        {
            ReachedTarget();
        }

        if ((bool)StandingStill)
        {
            standingStillTime += Time.deltaTime;
        }
        else
        {
            standingStillTime = 0;
        }

        if ((standingStillTime > 2.5f || (standingStillTime > 0.4f && Navigation.path.status != NavMeshPathStatus.PathComplete)) && DistanceToTargetSquared > personalBoundaryDistanceSquared)
        {
            standingStillTime = 0;
            ReachedTarget();
        }
    }

    private void ReachedTarget()
    {
        FindNewRoamTarget();
    }

    private void TargetWasUpdated()
    {
        Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

        if (Quaternion.Angle(Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), transform.rotation) > 1 && DistanceToTargetSquared > personalBoundaryDistanceSquared)
        {
            isFacingTarget = false;
            rotationLerpTime = 0;
        }
        else
        {
            StartedFacingTarget();
        }
    }

    private void StartedFacingTarget() //the character has rotated towards the target and will now start to move towards it
    {
        isFacingTarget = true;

        if (Navigation.isActiveAndEnabled)
            Navigation.SetDestination(TargetPosition);
    }

    private void FindNewRoamTarget()
    {
        Vector3? newPosition = GetRandomPositionWithinDistance(RoamNewTargetRange);

        if (newPosition != null)
        {
            SetNewTarget((Vector3)newPosition);
        }
    }

    private void SetNewTarget(Vector3 position)
    {
        TargetPosition = position;
    }

    private Vector3? GetRandomPositionWithinDistance(float distance, int attempt = 0, Vector3 offset = default(Vector3))
    {
        bool foundValue = false;

        Vector3 randomPosition = Random.insideUnitSphere * distance;
        randomPosition.y = transform.position.y;

        Vector3? result = transform.position + randomPosition;
        if (offset != default(Vector3))
            result += offset;

        result += Vector3.up * 0.5f;

        Ray groundRay = new Ray((Vector3)result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            if (groundHit.transform.tag == "Ground")
            {
                foundValue = true;
                result = groundHit.point;
            }
        }

        if (!foundValue)
        {
            if (attempt <= 10)
                result = GetRandomPositionWithinDistance(distance, attempt + 1); //try again
            else
                result = null; //give up
        }

        return result;
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        if (UniqueSoundSource.Active)
        {
            FootStepAudio.PlayNext();
        }
        else
        {
            Debug.Log("Unique sound not active");
        }
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        Debug.Log("robot attacked");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(TargetPosition, 1);
    }
}
