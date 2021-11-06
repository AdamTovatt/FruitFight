using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
    public float DistanceToGround;
    public float RaycastStartHeight = 0.4f;
    public float GroundedGameTimeLength = 0.2f;

    public Transform GroundTransform { get; private set; }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsGroundedWithGameTime
    {
        get
        {
            return isGrounded || Time.time - lastGroundedTime < GroundedGameTimeLength;
        }
    }

    public delegate void BecameGroundedHandler();
    public event BecameGroundedHandler OnBecameGrounded;

    private bool previousGrounded;
    private float lastGroundedTime;
    private bool isGrounded;

    private void Start()
    {
        CalculateIsGrounded();
    }

    private void Update()
    {
        previousGrounded = IsGrounded;
        CalculateIsGrounded();
    }

    private void CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * RaycastStartHeight, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.tag == "Ground" && hit.distance <= DistanceToGround)
            {
                GroundTransform = hit.transform;

                if (!previousGrounded)
                    OnBecameGrounded?.Invoke();

                lastGroundedTime = Time.time;
                isGrounded = true;
                return;
            }
        }

        GroundTransform = null;
        isGrounded = false;
    }
}
