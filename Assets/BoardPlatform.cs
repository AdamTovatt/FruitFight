using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlatform : MonoBehaviour
{
    public float AnimationSpeed = 1f;
    public float RotateAxisOffsetLength = 0.9f;
    public Collider Collider;

    public float UpTime = 0.4f;
    public float DownTime = 2f;

    private bool isUnderPressure;
    private float upTimeLeft;

    private bool isDown;
    private float downTimeLeft;

    private Vector3 rotateAxisOffset;
    private Vector3 rotatePoint;
    private Vector3 rotateAxis;

    private int rotateDirectionMultiplier = 1;

    private Quaternion startRotation;
    private Vector3 startPosition;

    private bool goingDown = false;
    private bool goingUp = false;

    private float goingDownProgress;
    private float goingUpProgress;

    private float lastRotationValue = -1;

    Block block;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        upTimeLeft = UpTime;
        downTimeLeft = DownTime;

        if (!WorldBuilder.IsInEditor)
        {
            block = gameObject.GetComponentInParent<BlockInformationHolder>().Block;

            if (block.NeighborX.Positive.Count > 0)
            {
                rotateAxisOffset += new Vector3(RotateAxisOffsetLength, 0, 0);
                rotateAxis = transform.forward;
                rotateDirectionMultiplier = 1;
            }
            else if (block.NeighborX.Negative.Count > 0)
            {
                rotateAxisOffset -= new Vector3(RotateAxisOffsetLength, 0, 0);
                rotateAxis = transform.forward;
                rotateDirectionMultiplier = -1;
            }
            else if (block.NeighborZ.Positive.Count > 0)
            {
                rotateAxisOffset += new Vector3(0, 0, RotateAxisOffsetLength);
                rotateAxis = transform.right;
                rotateDirectionMultiplier = -1;
            }
            else if (block.NeighborZ.Negative.Count > 0)
            {
                rotateAxisOffset -= new Vector3(0, 0, RotateAxisOffsetLength);
                rotateAxis = transform.right;
                rotateDirectionMultiplier = 1;
            }
            else
            {
                rotateAxisOffset += new Vector3(RotateAxisOffsetLength, 0, 0);
                rotateAxis = transform.forward;
                rotateDirectionMultiplier = 1;
            }

            rotatePoint = transform.position + rotateAxisOffset;
        }
    }

    private void Update()
    {
        if (!isDown)
        { //is up
            if (isUnderPressure)
            {
                upTimeLeft -= Time.deltaTime;
            }
            else
            {
                if (upTimeLeft < UpTime)
                    upTimeLeft += Time.deltaTime;
            }

            if (upTimeLeft < 0)
            {
                GoDown();
            }
        }
        else //is down
        {
            if (downTimeLeft < 0)
            {
                GoUp();
            }
            else
            {
                downTimeLeft -= Time.deltaTime;
            }
        }

        if (goingDown)
        {
            goingDownProgress += Time.deltaTime * AnimationSpeed;

            if (goingDownProgress < 2)
            {
                transform.rotation = Quaternion.identity;
                float rotateValue = Mathf.Abs(Mathf.Sin(Mathf.Pow((goingDownProgress + 0.4f) * 2, 1.75f))) * Mathf.Pow(2.718f, -1.7f * goingDownProgress * 2) * 1.6f;

                Vector3 newPosition = Vector3.zero;
                Quaternion newRotation = RotateAround(rotatePoint, rotateAxis, (90f - (rotateValue * 90f)) * rotateDirectionMultiplier, out newPosition);
                transform.position = newPosition;
                transform.rotation = newRotation;
            }
            else
            {
                goingDown = false;
                goingDownProgress = 2;
            }
        }
        else if (goingUp)
        {
            goingUpProgress += Time.deltaTime * AnimationSpeed;

            if (goingUpProgress < 1)
            {
                float rotateValue = goingUpProgress;

                Vector3 newPosition = Vector3.zero;
                Quaternion newRotation = RotateAround(rotatePoint, rotateAxis, (90f - (rotateValue * 90f)) * rotateDirectionMultiplier, out newPosition);
                transform.position = newPosition;
                transform.rotation = newRotation;
            }
            else
            {
                goingUp = false;
                goingUpProgress = 1;
            }
        }
    }

    private Quaternion RotateAround(Vector3 center, Vector3 axis, float angle, out Vector3 newPosition)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
        Vector3 dir = startPosition - center; // find current direction relative to center
        dir = rot * dir; // rotate the direction
        newPosition = center + dir; // define new position
        return startRotation * Quaternion.Inverse(startRotation) * rot * startRotation;
    }

    public void GoDown()
    {
        if (!isDown)
        {
            StopPressure();
            isDown = true;
            goingDown = true;
            goingDownProgress = 0;
        }
    }

    public void GoUp()
    {
        if (isDown)
        {
            downTimeLeft = DownTime;
            upTimeLeft = UpTime;
            isDown = false;
            goingUp = true;
            goingUpProgress = 0;
        }
    }

    public void StartPressure()
    {
        isUnderPressure = true;
    }

    public void StopPressure()
    {
        isUnderPressure = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            StartPressure();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            StopPressure();
        }
    }
}
