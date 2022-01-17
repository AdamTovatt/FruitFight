using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlatform : MonoBehaviour
{
    private const float downAnimationTime = 2f;
    private const float upAnimationTime = 1f;

    public float AnimationSpeed = 1f;
    public float RotateAxisOffsetLength = 0.9f;
    public Collider Collider;
    public SoundSource Sound;

    public float UpTime = 0.4f;
    public float DownTime = 2f;

    public float YDistanceMargin = 0.3f;

    public bool IsMoving { get { return goingUp || goingDown; } }

    private bool isUnderPressure;
    private float upTimeLeft;

    private bool isDown;
    private float downTimeLeft;
    private bool isUp = true;

    private Vector3 rotateAxisOffset;
    private Vector3 rotatePoint;
    private Vector3 rotateAxis;

    private int rotateDirectionMultiplier = 1;

    private Quaternion startRotation;
    private Vector3 startPosition;
    private Vector3 originalStartPosition;

    private bool goingDown = false;
    private bool goingUp = false;

    private float goingDownProgress;
    private float goingUpProgress;

    Block block;
    MoveOnTrigger moveOnTrigger;

    private bool queuedActivate;
    private bool queuedDeactivate;

    private void Start()
    {
        startPosition = transform.position;
        originalStartPosition = startPosition;
        startRotation = transform.rotation;
        upTimeLeft = UpTime;
        downTimeLeft = DownTime;

        if (!WorldBuilder.IsInEditor)
        {
            block = gameObject.GetComponentInParent<BlockInformationHolder>().Block;
            moveOnTrigger = gameObject.GetComponentInParent<MoveOnTrigger>();

            if (block.NeighborX.AllTypesPositive.Count > 0)
            {
                rotateAxisOffset += new Vector3(RotateAxisOffsetLength, 0, 0);
                rotateAxis = transform.forward;
                rotateDirectionMultiplier = 1;
            }
            else if (block.NeighborX.AllTypesNegative.Count > 0)
            {
                rotateAxisOffset -= new Vector3(RotateAxisOffsetLength, 0, 0);
                rotateAxis = transform.forward;
                rotateDirectionMultiplier = -1;
            }
            else if (block.NeighborZ.AllTypesPositive.Count > 0)
            {
                rotateAxisOffset += new Vector3(0, 0, RotateAxisOffsetLength);
                rotateAxis = transform.right;
                rotateDirectionMultiplier = -1;
            }
            else if (block.NeighborZ.AllTypesNegative.Count > 0)
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

            if (goingDownProgress < downAnimationTime)
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

            if (goingUpProgress < upAnimationTime)
            {
                float rotateValue = (Mathf.Sin(Mathf.PI * goingUpProgress - (Mathf.PI / 2f)) + 1f) / 2f;

                Vector3 newPosition;
                Quaternion newRotation = RotateAround(rotatePoint, rotateAxis, (90f - (rotateValue * 90f)) * rotateDirectionMultiplier, out newPosition);
                transform.position = newPosition;
                transform.rotation = newRotation;
            }
            else
            {
                goingUp = false;
                goingUpProgress = 1;
                CompletedRise();
            }
        }
    }

    public void QueueActivate()
    {
        if (moveOnTrigger.Active)
        {
            queuedActivate = false;
            queuedDeactivate = false;
        }
        else
        {
            queuedActivate = true;
            queuedDeactivate = false;
        }
    }

    public void QueueDeactivate()
    {
        if (moveOnTrigger.Active)
        {
            queuedDeactivate = true;
            queuedActivate = false;
        }
        else
        {
            queuedActivate = false;
            queuedDeactivate = false;
        }
    }

    private void CompletedRise()
    {
        isUp = true;

        if (moveOnTrigger != null)
        {
            if (queuedActivate && queuedDeactivate)
                Debug.LogError("Both activate and deactivate are queued at the same time! This should not be possible");

            if (queuedActivate)
            {
                moveOnTrigger.DoActivate();
            }
            else if (queuedDeactivate)
            {
                moveOnTrigger.DoDeActivate();
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
        if (moveOnTrigger != null && moveOnTrigger.Moving)
            return;

        if (!isDown)
        {
            if (moveOnTrigger != null)
            {
                if (isUp)
                {
                    if (moveOnTrigger.Active)
                    {
                        startPosition = transform.position;
                        rotatePoint = transform.position + rotateAxisOffset;
                    }
                    else
                    {
                        startPosition = originalStartPosition;
                        rotatePoint = originalStartPosition + rotateAxisOffset;
                    }
                }
            }

            StopPressure();
            isDown = true;
            goingDown = true;
            goingUp = false;
            isUp = false;

            if (goingUpProgress != 1 && goingUpProgress != 0)
            {
                goingDownProgress = (downAnimationTime / upAnimationTime) * (upAnimationTime - goingUpProgress) * 0.5f;
            }
            else
            {
                goingDownProgress = 0;
            }

            Sound.Play("Fall");
            Sound.StopPlaying("Rise");
        }
    }

    public void GoUp()
    {
        if (moveOnTrigger != null && moveOnTrigger.Moving)
            return;

        if (isDown)
        {
            downTimeLeft = DownTime;
            upTimeLeft = UpTime;
            isDown = false;
            goingUp = true;
            goingUpProgress = 0;

            Sound.Play("Rise");
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
            if (collision.transform.position.y + YDistanceMargin > transform.position.y || goingUp)
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
