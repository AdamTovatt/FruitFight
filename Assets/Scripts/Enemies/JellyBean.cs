using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JellyBean : MonoBehaviour
{
    public TextMeshPro StatusText;

    public float RoamSpeed = 1f;
    public float RoamNewTargetRange = 5f;
    public float PersonalBoundaryDistance = 0.1f;
    public float DiscoveryRadius = 4f;

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

    private Rigidbody _rigidbody;

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

    private Vector3 targetSubPosition;
    private Vector3 targetPosition;

    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);
        OnStateChanged += (sender, newState) => { JellyBeanStateWasChanged(newState); };
        State = Random.Range(1, 3) > 1 ? JellyBeanState.Idle : JellyBeanState.Roaming;
    }

    void Update()
    {
        StatusText.transform.rotation = Quaternion.LookRotation(StatusText.transform.position - GameManager.Instance.Camera.transform.position);

        if((GameManager.Instance.Players[0].transform.position - transform.position).sqrMagnitude < DiscoveryRadius)
        {
            targetPosition = GameManager.Instance.Players[0].transform.position;
            State = JellyBeanState.Chasing;
        }

        switch (State)
        {
            case JellyBeanState.Roaming:
                if (TimeInCurrentState < 5f + randomTimeAddition)
                {
                    if (Vector3.Distance(targetPosition, transform.position) < PersonalBoundaryDistance)
                    {
                        targetPosition = GetNewTargetPosition();
                    }

                    MoveTowardsTarget(RoamSpeed);
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

                }
                break;
            case JellyBeanState.Chasing:
                MoveTowardsTarget(RoamSpeed * 5);
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

    private void MoveTowardsTarget(float speed)
    {
        Vector3 targetDirection = (CurrentTargetPosition - transform.position);
        targetDirection.y = 0;
        targetDirection.Normalize();

        _rigidbody.MovePosition(transform.position + (targetDirection * speed * 5) * Time.deltaTime);
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

        Vector3 towardsTarget = result - transform.position;
        Ray visionOfTargetRay = new Ray(transform.position, towardsTarget);
        if (Physics.Raycast(visionOfTargetRay, out RaycastHit hit))
        {
            if ((hit.point - result).sqrMagnitude > 0.1)
            {
                Vector3 cross = Vector3.Cross(hit.normal, Vector3.up);

                right = hit.point + cross.normalized * 1;
                left = hit.point + cross.normalized * -1;

                if(VectorIsNegative(Vector3.Scale(cross.normalized * 1, towardsTarget)))
                {
                    result = right;
                }
                else
                {
                    result = left;
                }
            }
        }

        return result;
    }

    private bool VectorIsNegative(Vector3 vector)
    {
        if (vector.x < 0 || vector.y < 0 || vector.z < 0)
            return true;
        return false;
    }

    private Vector3 right;
    private Vector3 left;

    private void OnDrawGizmos()
    {
        if (targetPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.08f);
        }

        if (right != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(right, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(left, 0.1f);
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
                targetPosition = GetNewTargetPosition();
                break;
            case JellyBeanState.Idle:
                break;
            case JellyBeanState.Chasing:
                break;
            case JellyBeanState.Confused:
                break;
            case JellyBeanState.Searching:
                break;
            default:
                throw new System.Exception("No such state for: " + ToString());
        }
    }
}

public enum JellyBeanState
{
    Roaming, Idle, Chasing, Confused, Searching
}
