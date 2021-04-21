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

    private Rigidbody rigidbody;

    private float lastStateChange;
    private float randomTimeAddition;
    private bool remembersPlayer = false;

    private float randomTimeMin = 0;
    private float randomTimeMax = 0;

    private Vector3 targetPosition;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);
        OnStateChanged += (sender, newState) => { JellyBeanStateWasChanged(newState); };
        State = Random.Range(1, 3) > 1 ? JellyBeanState.Idle : JellyBeanState.Roaming;
    }

    void Update()
    {
        StatusText.transform.rotation = Quaternion.LookRotation(StatusText.transform.position - GameManager.Instance.Camera.transform.position);

        switch (State)
        {
            case JellyBeanState.Roaming:
                if (TimeInCurrentState < 5f + randomTimeAddition)
                {
                    Debug.Log(Vector3.Distance(targetPosition, transform.position));
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
        Vector3 targetDirection = (targetPosition - transform.position);
        targetDirection.y = 0;
        targetDirection.Normalize();

        Ray visionOfTargetRay = new Ray(transform.position, targetPosition);
        if(Physics.Raycast(visionOfTargetRay, out RaycastHit hit))
        {
            if((hit.point - targetPosition).sqrMagnitude > 0.1)
            {

            }
        }

        rigidbody.MovePosition(transform.position + (targetDirection * speed * 5) * Time.deltaTime);
    }

    private Vector3 GetNewTargetPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * RoamNewTargetRange;
        randomPosition.y = transform.position.y;

        Vector3 result = transform.position + randomPosition;

        result += Vector3.up * 0.05f;

        Ray groundRay = new Ray(result, Vector3.up * -1);
        if(Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            result = groundHit.point;
        }
        else
        {
            result.y = transform.position.y;
        }

        return result;
    }

    private void OnDrawGizmos()
    {
        if(targetPosition != Vector3.zero)
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
