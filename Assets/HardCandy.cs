using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HardCandy : MovingCharacter
{
    public NavMeshAgent Navigation;

    public float PersonalBoundaryDistance = 0.7f;
    public float RoamNewTargetRange = 5f;
    public float RoamSpeed = 1f;
    public float ChargeSpeed = 2f;

    public float DistanceToGround = 0.1f;

    public UniqueSoundSource UniqueSoundSource;
    public FootStepAudioSource FootStepAudio;
    public SoundSource Sound;
    public Health Health;

    public float CurrentMaxSpeed { get; set; }
    public Vector3 TargetPosition { get { return _targetPosition; } set { if (Navigation.isActiveAndEnabled) Navigation.SetDestination(value); _targetPosition = value; } }
    private Vector3 _targetPosition;

    public override bool StopFootSetDefault { get { return false; } }

    public override bool IsGrounded
    {
        get
        {
            if (_isGrounded == null)
                _isGrounded = CalculateIsGrounded();
            return (bool)_isGrounded;
        }
    }
    private bool? _isGrounded = null;

    public override bool? StandingStill { get { return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    private float personalBoundaryDistanceSquared;

    private void Awake()
    {
        BindEvents();
        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;
    }

    private void Start()
    {
        SetNewRoamTarget();
    }

    private void Update()
    {
        Navigation.speed = CurrentMaxSpeed;

        if ((TargetPosition - transform.position).sqrMagnitude < personalBoundaryDistanceSquared)
        {
            ReachedTarget();
        }

        _isGrounded = null; //reset so it will be calculated the next frame if needed
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        Health.OnDied += Died;
    }

    private void UnBindEvents()
    {
        Health.OnDied -= Died;
    }

    private void ReachedTarget()
    {
        //if(roaming) {
        SetNewRoamTarget();
        //} later maybe
    }

    private void SetNewRoamTarget()
    {
        Vector3? newPosition = GetRandomPositionWithinDistance(RoamNewTargetRange);

        if (newPosition != null)
        {
            TargetPosition = (Vector3)newPosition;
            CurrentMaxSpeed = RoamSpeed;
        }
        else
        {
            Debug.LogError("Hard Candy could not find new target position");
        }
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        Debug.Log("Hard candy died :(");
        Destroy(gameObject);
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        Debug.Log("Hard candy was attacked by " + attackingTransform.name);
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        if (UniqueSoundSource.Active)
        {
            FootStepAudio.PlayNext();
        }
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;
        }

        return false;
    }

    private Vector3? GetRandomPositionWithinDistance(float distance, int attempt = 0)
    {
        bool foundValue = false;

        Vector3 randomPosition = Random.insideUnitSphere * distance;
        randomPosition.y = transform.position.y;

        Vector3? result = transform.position + randomPosition;

        result += Vector3.up * 0.05f;

        Ray groundRay = new Ray((Vector3)result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            if (groundHit.transform.tag == "Ground")
            {
                foundValue = true;
                result = groundHit.point;
                Debug.Log("Groundray hit: " + groundHit.transform.name);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(TargetPosition, new Vector3(0.4f, 0.2f, 0.4f));
    }
}
