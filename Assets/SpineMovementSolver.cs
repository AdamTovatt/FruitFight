using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineMovementSolver : MonoBehaviour
{
    public IkFootSolver RightFoot;
    public IkFootSolver LeftFoot;
    public MovingCharacter Character;

    public float BounceSpeed = 1f;
    public float BounceHeight = 1f;
    public float CharacterMaxVelocity = 4f;

    public Vector3 CurrentPosition { get; private set; }
    public Vector3 NewPosition { get; private set; }
    public Vector3 OldPosition { get; private set; }

    private Vector3 distanceToCharacterTransform;
    private float lerp = 1;

    private AverageVelocityKeeper characterVelocity;

    void Start()
    {
        distanceToCharacterTransform = transform.position - Character.transform.position;
        characterVelocity = Character.GetComponent<AverageVelocityKeeper>();
    }

    void Update()
    {
        transform.position = CurrentPosition + distanceToCharacterTransform + Character.transform.position;

        if(lerp < 1 && Character.IsGrounded)
        {
            Vector3 spinePosition = Character.transform.up * (Mathf.Sin(lerp * Mathf.PI) - 0.5f) * BounceHeight * (characterVelocity.Velocity / CharacterMaxVelocity);

            CurrentPosition = spinePosition;
            lerp += Time.deltaTime * BounceSpeed * characterVelocity.Velocity;
        }
        else
        {
            lerp = 0;
            OldPosition = NewPosition;
        }
    }
}
