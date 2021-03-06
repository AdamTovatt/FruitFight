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

    public bool GroundTransformIsBouncy { get { return bouncyObjects.Contains(GroundTransform); } }

    public Block GroundBlock { get { return GetBlock(); } }

    public delegate void BecameGroundedHandler();
    public event BecameGroundedHandler OnBecameGrounded;

    public delegate void NewGroundWasEnteredHandler(Block newGround);
    public event NewGroundWasEnteredHandler OnNewGroundWasEntered;

    private bool previousGrounded;
    private float lastGroundedTime;
    private bool isGrounded;
    private Block lastBlock;

    private List<Transform> bouncyObjects = new List<Transform>();
    private Dictionary<Transform, Block> blockDictionary = new Dictionary<Transform, Block>();

    private void Start()
    {
        CalculateIsGrounded();

        foreach (BouncyObject bouncyObject in FindObjectsOfType<BouncyObject>())
        {
            bouncyObjects.Add(bouncyObject.transform);
        }
    }

    private void Update()
    {
        previousGrounded = IsGrounded;
        CalculateIsGrounded();
        GetBlock();
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

    private Block GetBlock()
    {
        Block currentBlock;
        if (GroundTransform != null)
        {
            if (!blockDictionary.ContainsKey(GroundTransform))
            {
                BlockInformationHolder blockInformationHolder = GroundTransform.GetComponentInParent<BlockInformationHolder>();
                if (blockInformationHolder == null)
                    blockInformationHolder = GroundTransform.GetComponent<BlockInformationHolder>();

                blockDictionary.Add(GroundTransform, blockInformationHolder == null ? null : blockInformationHolder.Block);
            }

            currentBlock = blockDictionary[GroundTransform];
        }
        else
        {
            currentBlock = null;
        }

        if(currentBlock != lastBlock)
        {
            OnNewGroundWasEntered?.Invoke(currentBlock);
        }

        lastBlock = currentBlock;

        return currentBlock;
    }
}
