using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class JellyBean : MovingCharacter
{
    public TextMeshPro StatusText;

    public float RoamSpeed = 1f;
    public float RoamNewTargetRange = 5f;
    public float ChaseSpeed = 3f;
    public float PersonalBoundaryDistance = 0.1f;
    public float DiscoveryRadius = 4f;
    public float DistanceToGround = 0.1f;

    public JellyBeanState State
    {
        get { return _state; }
        private set { OnStateChanged?.Invoke(this, value); _state = value; }
    }

    private JellyBeanState _state;

    public float TimeInCurrentState
    {
        get
        {
            return Time.time - lastStateChange;
        }
    }

    public delegate void StateChangedHandler(object sender, JellyBeanState newState);
    public event StateChangedHandler OnStateChanged;
    public override event AttackHandler OnPunched;

    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidbody;

    private float lastStateChange;
    private float randomTimeAddition;
    private bool remembersPlayer = false;

    private float randomTimeMin = 0;
    private float randomTimeMax = 0;

    private Vector3 CurrentTargetPosition
    {
        get
        {
            if (targetSubPosition == Vector3.zero)
                return targetPosition;
            return targetSubPosition;
        }
    }

    public override bool IsGrounded
    {
        get
        {
            if (_isGrounded == null)
                _isGrounded = CalculateIsGrounded();
            Debug.Log((bool)_isGrounded);
            return (bool)_isGrounded;
        }
    }
    private bool? _isGrounded = null;

    public override bool StandingStill { get { return navMeshAgent.velocity.sqrMagnitude == 0 || navMeshAgent.isStopped; } }

    private Vector3 targetSubPosition;
    private Vector3 targetPosition;
    private Transform target;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);
        OnStateChanged += (sender, newState) => { JellyBeanStateWasChanged(newState); };
        State = Random.Range(1, 3) > 1 ? JellyBeanState.Idle : JellyBeanState.Roaming;

        rigidbody.isKinematic = true;
    }

    void Update()
    {
        StatusText.transform.rotation = Quaternion.LookRotation(StatusText.transform.position - GameManager.Instance.Camera.transform.position);

        if (target == null)
        {
            PlayerMovement closestPlayer = null;
            float closestPlayerDistance = 100000;

            foreach (PlayerMovement player in GameManager.Instance.Players)
            {
                float distanceSquared = (player.transform.position - transform.position).sqrMagnitude;
                if (distanceSquared < DiscoveryRadius * DiscoveryRadius)
                {
                    if (closestPlayerDistance > distanceSquared)
                    {
                        closestPlayer = player;
                        closestPlayerDistance = distanceSquared;
                    }
                }
            }

            if (closestPlayer != null)
            {
                target = closestPlayer.transform;
                State = JellyBeanState.Chasing;
            }
        }

        if (target != null)
        {
            if ((target.position - transform.position).sqrMagnitude < DiscoveryRadius * DiscoveryRadius * 2)
            {
                targetPosition = target.position;
            }
            else
            {
                target = null;
                State = JellyBeanState.Confused;
            }
        }

        switch (State)
        {
            case JellyBeanState.Roaming:
                if (TimeInCurrentState < 5f + randomTimeAddition)
                {
                    if (Vector3.Distance(targetPosition, transform.position) < PersonalBoundaryDistance)
                    {
                        State = JellyBeanState.Roaming;
                    }

                    navMeshAgent.SetDestination(targetPosition);
                }
                else
                {
                    if (Random.Range(0, 10) < 2)
                        State = JellyBeanState.Confused;
                    else
                        State = JellyBeanState.Idle;
                }
                break;
            case JellyBeanState.Idle:
                if (TimeInCurrentState > 2f + randomTimeAddition)
                {
                    if (remembersPlayer)
                    {
                        State = JellyBeanState.Searching;
                    }
                    else
                    {
                        State = JellyBeanState.Roaming;
                    }
                }
                else
                {
                    //idle
                }
                break;
            case JellyBeanState.Chasing:
                navMeshAgent.SetDestination(targetPosition);
                break;
            case JellyBeanState.Confused:
                State = JellyBeanState.Idle;
                break;
            case JellyBeanState.Searching:
                break;
            default:
                break;
        }
    }

    private Vector3 GetNewTargetPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * RoamNewTargetRange;
        randomPosition.y = transform.position.y;

        Vector3 result = transform.position + randomPosition;

        result += Vector3.up * 0.05f;

        Ray groundRay = new Ray(result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            result = groundHit.point;
        }
        else
        {
            result.y = transform.position.y;
        }

        return result;
    }

    private bool HasVisionOfTransform(Transform target)
    {
        Ray ray = new Ray(transform.position, transform.position - target.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == target;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (targetPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.08f);
        }
    }

    public void JellyBeanStateWasChanged(JellyBeanState newState)
    {
        lastStateChange = Time.time;
        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);

        StatusText.text = newState.ToString();

        switch (newState)
        {
            case JellyBeanState.Roaming:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = RoamSpeed;
                targetPosition = GetNewTargetPosition();
                break;
            case JellyBeanState.Idle:
                navMeshAgent.isStopped = true;
                break;
            case JellyBeanState.Chasing:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = ChaseSpeed;
                break;
            case JellyBeanState.Confused:
                navMeshAgent.isStopped = true;
                break;
            case JellyBeanState.Searching:
                navMeshAgent.isStopped = false;
                break;
            default:
                throw new System.Exception("No such state for: " + ToString());
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
}

public enum JellyBeanState
{
    Roaming, Idle, Chasing, Confused, Searching
}
