using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 move;
    public Rigidbody RigidBody;

    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float DistanceToGround = 0.25f;

    public Transform Camera;

    public bool IsGrounded
    {
        get
        {
            if (_isGrounded == null)
                _isGrounded = CalculateIsGrounded();
            return (bool)_isGrounded;
        }
    }
    private bool? _isGrounded = null;
    private float distanceToGround;

    private void Awake()
    {
        controls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();
        distanceToGround = gameObject.GetComponent<Collider>().bounds.extents.y + 1f;

        controls.Gameplay.Attack.performed += (context) => { Attack(); };
        controls.Gameplay.Jump.performed += (context) => { Jump(); };
        controls.Gameplay.Move.performed += (context) => { move = context.ReadValue<Vector2>(); };
        controls.Gameplay.Move.canceled += (controls) => { move = Vector2.zero; };
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
        if (newPosition - transform.position != Vector3.zero)
            RigidBody.MoveRotation(Quaternion.LookRotation(newPosition - transform.position, Vector3.up));

        _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it

        RigidBody.AddForce(-Vector3.up);
    }

    private void Attack()
    {

    }

    private void Jump()
    {
        if (IsGrounded)
            RigidBody.AddForce(new Vector3(0, 50f * JumpStrength, 0));
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);

        RaycastHit hit;
        bool raycastResult = Physics.Raycast(ray, out hit);

        return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;
    }
}
