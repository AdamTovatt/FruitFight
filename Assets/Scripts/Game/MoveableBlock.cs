using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : NetworkBehaviour
{
    public Rigidbody Rigidbody;
    public float GlideSpeed = 5f;
    public float Drag = 1f;

    private Vector3 currentDirection;
    private float velocity;

    private void Update()
    {
        if (velocity > 0)
        {
            Rigidbody.MovePosition(transform.position + currentDirection * Time.deltaTime * GlideSpeed * velocity);
            velocity -= Time.deltaTime * Drag;
        }
        else
            velocity = 0;
    }

    public void Push(Vector3 direction)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                PerformPush(direction);
            }
            else
            {
                PushCmd(direction);
            }
        }
        else
        {
            PerformPush(direction);
        }
    }

    private void PerformPush(Vector3 direction)
    {
        currentDirection = direction;
        velocity = 1;
    }

    [Command(requiresAuthority = false)]
    public void PushCmd(Vector3 direction)
    {
        PerformPush(direction);
    }
}
