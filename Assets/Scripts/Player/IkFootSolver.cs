using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkFootSolver : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public float FootSpacing = 0.3f;
    public bool RightFoot = false;
    public float StepDistance = 0.5f;
    public float MinStepDistance = 0.2f;
    public float StepHeight = 0.4f;
    public float StepSpeed = 10f;
    public float ForwardShiftDivider = 10f;
    public IkFootSolver OtherFoot;

    public bool IsMoving { get { return lerp < 1; } }

    private int terrainLayer;
    private Transform player;
    private float appliedFootSpacing;

    private Vector3 oldPosition;
    private Vector3 currentPosition;
    private Vector3 newPosition;

    private float lerp;

    AverageVelocityKeeper playerVelocity;

    void Start()
    {
        playerVelocity = PlayerMovement.gameObject.GetComponent<AverageVelocityKeeper>();
        player = PlayerMovement.transform;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        appliedFootSpacing = FootSpacing * (RightFoot ? 1f : -1f);

        currentPosition = GetGroundPosition(0);
        lerp = 1;
    }

    void Update()
    {
        transform.position = currentPosition;

        float appliedStepDistance = Mathf.Max(playerVelocity.Velocity * StepDistance * 0.3f, MinStepDistance);

        Vector3 searchPosition = GetGroundPosition(appliedStepDistance + playerVelocity.Velocity / ForwardShiftDivider);

        if(Vector3.Distance(newPosition, searchPosition) > appliedStepDistance && !OtherFoot.IsMoving && lerp >= 1)
        {
            lerp = 0;
            oldPosition = currentPosition;
            newPosition = searchPosition;
        }

        if(lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * StepHeight;

            currentPosition = footPosition;
            lerp += Time.deltaTime * StepSpeed;
        }
        else
        {
            oldPosition = newPosition;
        }
    }

    private Vector3 GetGroundPosition(float forward)
    {
        Ray ray = new Ray(player.position + (Vector3.up * 0.5f) + (player.right * appliedFootSpacing) + (player.forward * forward), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10))
            return info.point;

        throw new System.Exception("No ground position found for next footstep");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.05f);
    }
}
