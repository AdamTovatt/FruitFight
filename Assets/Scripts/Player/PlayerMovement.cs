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

    private void Awake()
    {
        ControlsEnabled = true;
        inputActions = new Dictionary<System.Guid, PlayerInputAction>();
        playerControls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();

        inputActions.Add(playerControls.Gameplay.Attack.id, PlayerInputAction.Attack);
        inputActions.Add(playerControls.Gameplay.Jump.id, PlayerInputAction.Jump);
        inputActions.Add(playerControls.Gameplay.Move.id, PlayerInputAction.Move);
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
                default:
                    throw new System.Exception("Unknown action: " + context.action.name);
            }
        }
    }

    private void Update()
    {
        if (Camera == null)
            Camera = GameManager.Instance.MultipleTargetCamera.transform;

        Vector3 cameraForward = Camera.forward;
        Vector3 cameraRight = Camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementX = (cameraRight * move.x * Speed) / 100f;
        Vector3 movementY = (cameraForward * move.y * Speed) / 100f;

        Vector3 newPosition = RigidBody.transform.position + movementX + movementY;

        Ray forwardRay = new Ray(transform.position + transform.up * PunchHeight, transform.forward);
        if (!Physics.SphereCast(forwardRay, 0.4f, 0.4f, ~3))
        {
            RigidBody.MovePosition(newPosition);
        }

        if ((newPosition - transform.position != Vector3.zero) && move != Vector2.zero)
            RigidBody.MoveRotation(Quaternion.LookRotation(newPosition - transform.position, Vector3.up));

        _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it

        RigidBody.AddForce(-Vector3.up);
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
                        health.CurrentHealth -= PunchStrength;

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
    Attack, Jump, Move
}