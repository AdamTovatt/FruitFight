using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovingCharacter
{
    private PlayerControls controls;
    private Vector2 move;
    public Rigidbody RigidBody;
    private Transform Camera;

    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float DistanceToGround = 0.25f;
    public float MovementInputCutoff = 0.2f;
    public float PunchDistance = 0.5f;
    public float PunchHeight = 0.5f;
    public float PunchWidth = 0.5f;

    public override event AttackHandler OnPunched;

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

    private void Awake()
    {
        controls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();

        controls.Gameplay.Attack.performed += (context) => { Punch(); };
        controls.Gameplay.Jump.performed += (context) => { Jump(); };
        controls.Gameplay.Move.performed += (context) => { Move(context.ReadValue<Vector2>()); };
        controls.Gameplay.Move.canceled += (context) => { Move(Vector2.zero); };
    }

    private void Start()
    {
        Camera = GameManager.Instance.Camera.transform;
    }

    private void Update()
    {
        Vector3 cameraForward = Camera.forward;
        Vector3 cameraRight = Camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementX = (cameraRight * move.x * Speed) / 100f;
        Vector3 movementY = (cameraForward * move.y * Speed) / 100f;

        Vector3 newPosition = RigidBody.transform.position + movementX + movementY;

        RigidBody.MovePosition(newPosition);

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
        Vector3? punchTargetPoint = FindPunchTargetPoint();

        if(punchTargetPoint == null)
        {
            AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
            Vector3 punchPosition = transform.position + transform.forward * PunchDistance + transform.up * PunchHeight;
            punchPosition += transform.right * PunchWidth * (side == AttackSide.Right ? 1f : -1f);
            OnPunched?.Invoke(this, punchPosition, side);
        }
    }

    private Vector3? FindPunchTargetPoint()
    {
        return null;
    }

    private void Jump()
    {
        if (IsGrounded)
            RigidBody.AddForce(new Vector3(0, 50f * JumpStrength, 0));
    }

    private void OnEnable()
    {
        controls?.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.4f, -Vector3.up);

        RaycastHit hit;
        bool raycastResult = Physics.Raycast(ray, out hit);

        return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;
    }
}
