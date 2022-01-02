using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkArmSolver : MonoBehaviour
{
    public IkFootSolver IkFootSolver;
    public MovingCharacter CharacterMovement;
    public TrailRenderer Trail;

    public bool RightArm = false;
    public float ArmDistanceToBody = 0.5f;
    public float ArmHeight = 1.5f;
    public float ArmSwingSpeed = 1.2f;
    public float ArmForward = 1.2f;

    public Vector3 CurrentPosition { get; private set; }
    public Vector3 NewPosition { get; private set; }
    public Vector3 OldPosition { get; private set; }

    private PlayerNetworkCharacter networkCharacter;

    private float lerp = 1;
    private bool punching = false;
    private bool casting = false;
    private Vector3 punchStartPosition;
    private Vector3 punchEndPosition;
    private Vector3 characterPositionAtStartPunch;
    private Vector3 castPosition;
    private float castingSize = 0.2f;

    private bool holdingItem = false;
    private Holdable heldItem = null;

    private Player player;

    void Start()
    {
        if (CharacterMovement.transform.tag == "Player")
        {
            networkCharacter = CharacterMovement.gameObject.GetComponent<PlayerNetworkCharacter>();

            if (networkCharacter != null)
                networkCharacter.OnAttack += Punch;
        }

        CharacterMovement.OnAttack += Punch;

        if (CharacterMovement.GetType() == typeof(PlayerMovement))
        {
            player = CharacterMovement.gameObject.GetComponent<Player>();
            player.OnPickedUpItem += PickUpItem;
            player.OnDroppedItem += DropItem;
            player.OnStartedCasting += StartCasting;
            player.OnStoppedCasting += StopCasting;
        }

        if (Trail != null)
            Trail.enabled = false;
    }

    void Update()
    {
        if (!casting)
        {
            if (!holdingItem)
            {
                Vector3 swingPosition = CharacterMovement.transform.position + (IkFootSolver.OtherFoot.CurrentPosition - CharacterMovement.transform.position);
                swingPosition += CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody);
                swingPosition += CharacterMovement.transform.forward * ArmForward;
                swingPosition += CharacterMovement.transform.up * ArmHeight;

                if (!punching)
                {
                    CurrentPosition = swingPosition;
                    transform.position = CurrentPosition;
                }
                else
                {
                    Vector3 characterOffsetSincePunchStart = CharacterMovement.transform.position - characterPositionAtStartPunch;
                    punchEndPosition += characterOffsetSincePunchStart;

                    if (lerp < 3)
                    {
                        if (lerp < 1)
                            CurrentPosition = Vector3.Lerp(punchStartPosition, punchEndPosition, lerp);
                        else if (lerp > 1 && lerp < 2)
                            CurrentPosition = punchEndPosition;
                        else
                            CurrentPosition = Vector3.Lerp(punchEndPosition, swingPosition, lerp - 2);

                        transform.position = CurrentPosition;

                        lerp += Time.deltaTime * ArmSwingSpeed;
                    }
                    else
                    {
                        if (Trail != null)
                            Trail.enabled = false;
                        punching = false;
                    }
                }
            }
            else
            {
                transform.position = heldItem.HeldPosition + CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody) * heldItem.Radius;
            }
        }
        else
        {
            transform.position = CharacterMovement.transform.position + transform.forward * 0.5f + castPosition + CharacterMovement.transform.right * (RightArm ? ArmDistanceToBody : -1f * ArmDistanceToBody) * castingSize;
        }
    }

    private void StartCasting(Vector3 castingPosition, float castingSize)
    {
        this.castingSize = castingSize;
        castPosition = castingPosition;
        casting = true;
    }

    private void StopCasting()
    {
        casting = false;
    }

    private void Punch(Vector3 position, MovingCharacter.AttackSide attackSide)
    {
        if (RightArm && attackSide == MovingCharacter.AttackSide.Left)
            return;
        if (!RightArm && attackSide == MovingCharacter.AttackSide.Right)
            return;

        lerp = 0;
        punchStartPosition = CurrentPosition;
        punchEndPosition = position;
        characterPositionAtStartPunch = CharacterMovement.transform.position;

        if (Trail != null)
            Trail.enabled = true;

        punching = true;
    }

    private void PickUpItem(Holdable holdable)
    {
        holdingItem = true;
        heldItem = holdable;
    }

    private void DropItem()
    {
        holdingItem = false;
        heldItem = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentPosition, 0.05f);
    }

    private void OnDestroy()
    {
        if (CharacterMovement != null)
            CharacterMovement.OnAttack -= Punch;

        if (networkCharacter != null)
            networkCharacter.OnAttack -= Punch;

        if (player != null)
        {
            player.OnPickedUpItem -= PickUpItem;
            player.OnDroppedItem -= DropItem;
            player.OnStartedCasting -= StartCasting;
            player.OnStoppedCasting -= StopCasting;
        }
    }
}
