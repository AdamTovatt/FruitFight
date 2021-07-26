using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovingCharacter
{
    private Vector2 move;
    public Rigidbody RigidBody;
    private Transform Camera;
    private MultipleTargetCamera multipleTargetCamera;
    private PlayerControls playerControls;
    private Dictionary<System.Guid, PlayerInputAction> inputActions;

    public Transform PunchSphereTransform;
    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float DistanceToGround = 0.25f;
    public float MovementInputCutoff = 0.2f;
    public float PunchDistance = 0.5f;
    public float PunchHeight = 0.5f;
    public float PunchWidth = 0.5f;
    public float PunchSphereRadius = 0.4f;
    public float PunchStrength = 5f;
    public float RotateCameraSpeed = 5f;

    public bool ControlsEnabled { get; set; }

    public int JellyBeans { get; set; }

    public override event AttackHandler OnAttack;

    public override bool StopFootSetDefault { get { return false; } }
    public override bool StandingStill { get { return move == Vector2.zero; } }

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
    private float lastJumpTime;
    private Collision lastCollision;
    private float rotateCamera;

    private void Awake()
    {
        ControlsEnabled = true;
        inputActions = new Dictionary<System.Guid, PlayerInputAction>();
        playerControls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();

        inputActions.Add(playerControls.Gameplay.Attack.id, PlayerInputAction.Attack);
        inputActions.Add(playerControls.Gameplay.Jump.id, PlayerInputAction.Jump);
        inputActions.Add(playerControls.Gameplay.Move.id, PlayerInputAction.Move);
        inputActions.Add(playerControls.Gameplay.RotateCameraLeft.id, PlayerInputAction.RotateCameraLeft);
        inputActions.Add(playerControls.Gameplay.RotateCameraRight.id, PlayerInputAction.RotateCameraRight);
    }

    public void InitializePlayerInput(PlayerConfiguration playerConfiguration)
    {
        playerConfiguration.Input.onActionTriggered += HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (ControlsEnabled)
        {
            if (!inputActions.ContainsKey(context.action.id))
                throw new System.Exception("Unknown action: " + context.action.name);

            PlayerInputAction action = inputActions[context.action.id];

            switch (action)
            {
                case PlayerInputAction.Attack:
                    if (context.performed)
                        Punch();
                    break;
                case PlayerInputAction.Jump:
                    if (context.performed)
                        Jump();
                    break;
                case PlayerInputAction.Move:
                    if (context.performed)
                        Move(context.ReadValue<Vector2>());
                    else
                        Move(Vector2.zero);
                    break;
                case PlayerInputAction.RotateCameraLeft:
                    if (context.performed)
                        rotateCamera = -RotateCameraSpeed;
                    else
                        rotateCamera = 0;
                    break;
                case PlayerInputAction.RotateCameraRight:
                    if (context.performed)
                        rotateCamera = RotateCameraSpeed;
                    else
                        rotateCamera = 0;
                    break;
                default:
                    throw new System.Exception("Unknown action: " + context.action.name);
            }
        }
    }

    private List<Vector3> debugHits = new List<Vector3>();

    private void Update()
    {
        if (Camera == null)
        {
            Camera = GameManager.Instance.MultipleTargetCamera.transform;
            multipleTargetCamera = Camera.gameObject.GetComponent<MultipleTargetCamera>();
        }

        if (multipleTargetCamera != null)
            multipleTargetCamera.RotateCamera(rotateCamera);

        Vector3 cameraForward = Camera.forward;
        Vector3 cameraRight = Camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementX = (cameraRight * move.x * Speed) / 100f;
        Vector3 movementY = (cameraForward * move.y * Speed) / 100f;

        Vector3 movement = movementX + movementY;

        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(forwardRay, 0.4f, 0.4f, ~3);

        debugHits.Clear();

        foreach (RaycastHit hit in hits.Where(x => x.transform.position.y > transform.position.y + 0.3f))
        {
            debugHits.Add(hit.point);
            Vector3 wallDirection = new Vector3(hit.normal.z, hit.normal.y, hit.normal.x * -1).normalized;
            movement = Vector3.Dot(wallDirection, movement) * wallDirection;
        }

        Vector3 newPosition = RigidBody.transform.position + movement;

        if (!(hits.Where(x => x.transform.position.y > transform.position.y + 0.3f).Count() > 0)) //needs to be cast in a way that it hits even small blocks if the player is in the air
        {
            RigidBody.MovePosition(newPosition);
        }

        if ((newPosition - transform.position != Vector3.zero) && move != Vector2.zero) //rotate the player towards where it's going
            RigidBody.MoveRotation(Quaternion.LookRotation(movementX + movementY, Vector3.up));

        _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it

        RigidBody.AddForce(-Vector3.up * Time.deltaTime * 250); //make the player fall faster because the default fall rate is to slow
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Vector3 pos in debugHits)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }

    private void Move(Vector2 moveValue)
    {
        if (moveValue.magnitude > MovementInputCutoff)
            move = moveValue;
        else
            move = Vector2.zero;
    }

    private void Punch()
    {
        AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
        Vector3 punchPosition = transform.position + transform.forward * PunchDistance + transform.up * PunchHeight;
        punchPosition += transform.right * PunchWidth * (side == AttackSide.Right ? 1f : -1f);
        OnAttack?.Invoke(this, punchPosition, side);

        List<Transform> hits = new List<Transform>();
        CustomPhysics.ConeCastAll(transform.position + (transform.up * DistanceToGround), 2f, transform.forward, 1f, 25f).ForEach(x => hits.Add(x.transform));
        Physics.OverlapSphere(PunchSphereTransform.position, PunchSphereRadius).ToList().ForEach(x => hits.Add(x.transform));

        List<Transform> checkedTransforms = new List<Transform>();
        foreach (Transform hit in hits)
        {
            if (hit != this.transform)
            {
                if (!checkedTransforms.Contains(hit.transform))
                {
                    checkedTransforms.Add(hit.transform);

                    Health health = hit.transform.GetComponent<Health>();

                    if (health != null)
                        health.TakeDamage(PunchStrength);

                    JellyBean jellyBean = hit.transform.GetComponent<JellyBean>();
                    if (jellyBean != null)
                        jellyBean.WasAttacked(transform.position, transform, PunchStrength);
                }
            }
        }
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime > 0.2f && IsGrounded)
        {
            RigidBody.velocity = new Vector3(RigidBody.velocity.x, RigidBody.velocity.y + JumpStrength, RigidBody.velocity.z);
            lastJumpTime = Time.time;
        }
    }

    private void OnEnable()
    {
        ControlsEnabled = true;
    }

    private void OnDisable()
    {
        ControlsEnabled = false;
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.4f, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;

        return false;
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        throw new System.NotImplementedException();
    }

    public void AbsorbedItem(AbsorbableItemType itemType)
    {
        switch (itemType)
        {
            case AbsorbableItemType.JellyBean:
                JellyBeans++;
                Debug.Log("Jelly beans: " + JellyBeans);
                break;
            default:
                break;
        }
    }
}

public enum PlayerInputAction
{
    Attack, Jump, Move, RotateCameraRight, RotateCameraLeft
}