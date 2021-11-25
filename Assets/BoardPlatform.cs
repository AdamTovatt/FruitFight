using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlatform : MonoBehaviour
{
    public Transform RotatePoint;
    public Collider Collider;

    public float UpTime = 0.4f;
    public float DownTime = 2f;

    private bool isUnderPressure;
    private float upTimeLeft;

    private bool isDown;
    private float downTimeLeft;

    private void Start()
    {
        upTimeLeft = UpTime;
        downTimeLeft = DownTime;

        Debug.Log("Neighbours: " + gameObject.GetComponentInParent<BlockInformationHolder>().Block.NeighborX.Positive.Count.ToString());
    }

    private void Update()
    {
        if (!isDown)
        { //is up
            if (isUnderPressure)
            {
                upTimeLeft -= Time.deltaTime;
                Debug.Log(upTimeLeft);
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
    }

    public void GoDown()
    {
        if (!isDown)
        {
            StopPressure();
            isDown = true;
            transform.RotateAround(RotatePoint.position, transform.right, 90f);
        }
    }

    public void GoUp()
    {
        if (isDown)
        {
            downTimeLeft = DownTime;
            isDown = false;
            transform.RotateAround(RotatePoint.position, transform.right, -90f);
        }
    }

    public void StartPressure()
    {
        Debug.Log("Start");
        isUnderPressure = true;
    }

    public void StopPressure()
    {
        Debug.Log("Stop");
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
