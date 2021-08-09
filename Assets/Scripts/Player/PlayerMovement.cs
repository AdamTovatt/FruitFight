using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovingCharacter
{
    private Vector2 move;
    public Rigidbody RigidBody;
    private Transform Camera;
    private MultipleTargetCamera multipleTargetCamera;
    private PlayerControls playerControls;
    private Dictionary<System.Guid, PlayerInputAction> inputActions;
    private Health health;
    private AverageVelocityKeeper averageVelocityKeeper;
    private FootStepAudioSource footStepAudioSource;
    private SoundSource soundSource;

    public Transform PunchSphereTransform;
    public Transform SpineTransform;
    public float Speed = 5f;
    public float JumpStrength = 5f;
    public float DistanceToGround = 0.25f;
    public float MovementInputCutoff = 0.2f;
    public float PunchDistance = 0.5f;
    public float PunchHeight = 0.5f;
    public float PunchWidth = 0.5f;
    public float PunchSphereRadius = 0.4f;
    public float PunchStrength = 5f;
    public float RotateCameraSpeed = 5f;

    public float CurrentRunSpeed { get; set; }
    public bool ControlsEnabled { get; set; }
    public Holdable HeldItem { get; private set; }

    public Collider Collider { get; set; }

    public int JellyBeans { get; set; }

    public override event AttackHandler OnAttack;

    public delegate void OnPickedUpItemHandler(Holdable holdableItem);
    public event OnPickedUpItemHandler OnPickedUpItem;

    public delegate void OnDroppedItemHandler();
    public event OnDroppedItemHandler OnDroppedItem;

    public delegate void OnParentUpdatedHandler(MoveOnTrigger moveOnTriggerParent);
    public event OnParentUpdatedHandler OnParentUpdated;

    public override bool StopFootSetDefault { get { return false; } }
    public override bool StandingStill { get { return move == Vector2.zero; } }

    public override bool IsGrounded
    {
        get
        {
            if (_isGrounded == null)
                _isGrounded = CalculateIsGrounded();
            return (bool)_isGrounded;
        }
    }
    private bool? _isGrounded = null;
    private bool previousGrounded = false;

    private Transform groundTransform;
    private Dictionary<Transform, MoveOnTrigger> moveOnTriggerLookup = new Dictionary<Transform, MoveOnTrigger>();

    private float lastJumpTime;
    private Collision lastCollision;
    private float rotateCamera;

    private Dictionary<Transform, GroundedPositionInformation> previousGroundedPositions;
    private GroundedPositionInformation lastGroundedPosition;

    private List<float> previousDeathTimes;

    private void Awake()
    {
        previousDeathTimes = new List<float>();
        previousGroundedPositions = new Dictionary<Transform, GroundedPositionInformation>();
        ControlsEnabled = true;
        inputActions = new Dictionary<System.Guid, PlayerInputAction>();
        playerControls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();
        Collider = gameObject.GetComponent<Collider>();
        health = gameObject.GetComponent<Health>();
        averageVelocityKeeper = gameObject.GetComponent<AverageVelocityKeeper>();
        footStepAudioSource = gameObject.GetComponent<FootStepAudioSource>();
        soundSource = gameObject.GetComponent<SoundSource>();

        CurrentRunSpeed = Speed;

        inputActions.Add(playerControls.Gameplay.Attack.id, PlayerInputAction.Attack);
        inputActions.Add(playerControls.Gameplay.Jump.id, PlayerInputAction.Jump);
        inputActions.Add(playerControls.Gameplay.Move.id, PlayerInputAction.Move);
        inputActions.Add(playerControls.Gameplay.RotateCameraLeft.id, PlayerInputAction.RotateCameraLeft);
        inputActions.Add(playerControls.Gameplay.RotateCameraRight.id, PlayerInputAction.RotateCameraRight);
        inputActions.Add(playerControls.Gameplay.Pause.id, PlayerInputAction.Pause);
    }

    private void Start()
    {
        health.OnDied += OnDied;
    }

    private void OnDestroy()
    {
        health.OnDied -= OnDied;
    }

    private void OnDied(Health sender, CauseOfDeath causeOfDeath)
    {
        if (causeOfDeath == CauseOfDeath.Water)
        {
            previousDeathTimes.Add(Time.time);
            if (previousDeathTimes.Count > 5)
                previousDeathTimes.RemoveAt(0);

            if (previousDeathTimes.Count >= 5 && previousDeathTimes.Max() - previousDeathTimes.Min() < 10) //if we have died 5 times within 10 seconds
            {
                if (previousGroundedPositions.Keys.Count > 1)
                {
                    previousGroundedPositions.Remove(previousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key);
                    previousDeathTimes.Clear();
                }

                transform.position = previousGroundedPositions[previousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key].Position;
            }
            else
            {
                transform.position = lastGroundedPosition.Position;
            }
        }
    }

    public void InitializePlayerInput(PlayerConfiguration playerConfiguration)
    {
        playerConfiguration.Input.onActionTriggered += HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (ControlsEnabled)
        {
            if (!inputActions.ContainsKey(context.action.id))
                throw new System.Exception("Unknown action: " + context.action.name);

            PlayerInputAction action = inputActions[context.action.id];

            switch (action)
            {
                case PlayerInputAction.Attack:
                    if (context.performed)
                        Interact();
                    break;
                case PlayerInputAction.Jump:
                    if (context.performed)
                        Jump();
                    break;
                case PlayerInputAction.Move:
                    if (context.performed)
                        Move(context.ReadValue<Vector2>());
                    else
                        Move(Vector2.zero);
                    break;
                case PlayerInputAction.RotateCameraLeft:
                    if (context.performed)
                        rotateCamera = -RotateCameraSpeed;
                    else
                        rotateCamera = 0;
                    break;
                case PlayerInputAction.RotateCameraRight:
                    if (context.performed)
                        rotateCamera = RotateCameraSpeed;
                    else
                        rotateCamera = 0;
                    break;
                case PlayerInputAction.Pause:
                    //don't do anything, this is handled in the GameManager
                    break;
                default:
                    throw new System.Exception("Unknown action: " + context.action.name);
            }
        }
    }

    private List<Vector3> debugHits = new List<Vector3>();

    private void Update()
    {
        if (Camera == null)
        {
            Camera = GameManager.Instance.MultipleTargetCamera.transform;
            multipleTargetCamera = Camera.gameObject.GetComponent<MultipleTargetCamera>();
        }

        if (multipleTargetCamera != null)
            multipleTargetCamera.RotateCamera(rotateCamera);

        Vector3 cameraForward = Camera.forward;
        Vector3 cameraRight = Camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementX = (cameraRight * move.x * CurrentRunSpeed) / 100f;
        Vector3 movementY = (cameraForward * move.y * CurrentRunSpeed) / 100f;

        Vector3 movement = movementX + movementY;

        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(forwardRay, 0.4f, 0.4f, ~3);

        debugHits.Clear();

        foreach (RaycastHit hit in hits.Where(x => x.transform.position.y > transform.position.y + 0.3f))
        {
            debugHits.Add(hit.point);
            Vector3 wallDirection = new Vector3(hit.normal.z, hit.normal.y, hit.normal.x * -1).normalized;
            movement = Vector3.Dot(wallDirection, movement) * wallDirection;
        }

        Vector3 newPosition = RigidBody.transform.position + movement;

        if (IsGrounded)
        {
            if (!moveOnTriggerLookup.ContainsKey(groundTransform))
                moveOnTriggerLookup.Add(groundTransform, groundTransform.GetComponentInParent<MoveOnTrigger>());

            MoveOnTrigger moveOnTrigger = moveOnTriggerLookup[groundTransform];

            if (moveOnTrigger != null && moveOnTrigger.Moving) //if we are on a moving platform
            { //we want to use special movement on moving platforms
                if (transform.parent != moveOnTrigger.transform)
                    OnParentUpdated?.Invoke(moveOnTrigger);

                transform.SetParent(moveOnTrigger.transform);
                averageVelocityKeeper.Parent = moveOnTrigger.AverageVelocityKeeper;
            }
            else //we are on normal ground
            {
                transform.parent = null;
                UpdateGroundedPosition();
                averageVelocityKeeper.Parent = null;
            }
        }
        else
        {
            transform.parent = null;
        }

        if (!(hits.Where(x => x.transform.position.y > transform.position.y + 0.3f).Count() > 0)) //needs to be cast in a way that it hits even small blocks if the player is in the air
        {
            RigidBody.MovePosition(newPosition);

            if (transform.parent != null) //if we are on a moveOnTrigger
                transform.position += movement * CurrentRunSpeed * 6 * Time.deltaTime; //we should move with transform.position since rigidbody movement doesn't work
        }

        if ((newPosition - transform.position != Vector3.zero) && move != Vector2.zero) //rotate the player towards where it's going
            RigidBody.MoveRotation(Quaternion.LookRotation(movementX + movementY, Vector3.up));

        previousGrounded = IsGrounded;
        _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it

        RigidBody.AddForce(-Vector3.up * Time.deltaTime * 250); //make the player fall faster because the default fall rate is to slow
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 pos in debugHits)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }

    private void Move(Vector2 moveValue)
    {
        if (moveValue.magnitude > MovementInputCutoff)
            move = moveValue;
        else
            move = Vector2.zero;
    }

    private void Interact()
    {
        bool shouldPunch = true;

        if (HeldItem == null)
        {
            Ray ray = new Ray(transform.position + transform.up * 0.5f, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit)) //this is to check if there is something to pick up
            {
                if (hit.distance <= PunchDistance * 1.4f) //we can pick up things at 1.4 times the punch distance, I just picked 1.4 because it seems to work fine
                {
                    if (hit.transform.tag == "Interactable")
                    {
                        Holdable holdable = hit.transform.GetComponent<Holdable>();

                        if (holdable == null)
                        {
                            HoldableDelegate holdableDelegate = hit.transform.GetComponentInParent<HoldableDelegate>();
                            if (holdableDelegate != null)
                                holdable = holdableDelegate.ContainedHoldable;
                        }

                        if (holdable != null)
                        {
                            shouldPunch = false;
                            holdable.WasPickedUp(SpineTransform, transform.position + transform.up * PunchHeight + transform.forward * PunchDistance * 1.1f); //1.1 is the multiplier for how far forward we should hold the item
                            HeldItem = holdable;
                            CurrentRunSpeed = Speed * 0.8f;
                            OnPickedUpItem?.Invoke(holdable);
                        }
                    }
                }
            }
        }
        else
        {
            HeldItem.WasDropped(RigidBody, averageVelocityKeeper.Velocity);
            HeldItem = null;
            CurrentRunSpeed = Speed;
            OnDroppedItem?.Invoke();
            shouldPunch = false;
        }

        if (shouldPunch)
            Punch();
    }

    private void Punch()
    {
        AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
        Vector3 punchPosition = transform.position + transform.forward * PunchDistance + transform.up * PunchHeight;
        punchPosition += transform.right * PunchWidth * (side == AttackSide.Right ? 1f : -1f);
        OnAttack?.Invoke(this, punchPosition, side);

        Instantiate(PunchSoundEffectPrefab, punchPosition, Quaternion.identity);

        List<Transform> hits = new List<Transform>();
        CustomPhysics.ConeCastAll(transform.position + (transform.up * DistanceToGround), 2f, transform.forward, 1f, 25f).ForEach(x => hits.Add(x.transform));
        Physics.OverlapSphere(PunchSphereTransform.position, PunchSphereRadius).ToList().ForEach(x => hits.Add(x.transform));

        bool didHit = false;
        List<Transform> checkedTransforms = new List<Transform>();
        foreach (Transform hit in hits)
        {
            if (hit != transform)
            {
                if (!checkedTransforms.Contains(hit.transform))
                {
                    checkedTransforms.Add(hit.transform);

                    Health health = hit.transform.GetComponent<Health>();

                    if (health != null)
                    {
                        health.TakeDamage(PunchStrength);
                        didHit = true;
                    }

                    JellyBean jellyBean = hit.transform.GetComponent<JellyBean>();
                    if (jellyBean != null)
                        jellyBean.WasAttacked(transform.position, transform, PunchStrength);
                }
            }
        }

        if (didHit)
        {
            soundSource.Play("punchHit");
        }
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime > 0.2f && IsGrounded)
        {
            RigidBody.velocity = new Vector3(RigidBody.velocity.x, RigidBody.velocity.y + JumpStrength, RigidBody.velocity.z);
            lastJumpTime = Time.time;

            StepWasTaken(transform.position); //two sounds effects because two feet
            StepWasTaken(transform.position);
        }
    }

    private void OnEnable()
    {
        ControlsEnabled = true;
    }

    private void OnDisable()
    {
        ControlsEnabled = false;
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.4f, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.tag == "Ground" && hit.distance <= DistanceToGround)
            {
                groundTransform = hit.transform;

                if (!previousGrounded)
                    JustLanded();

                return true;
            }
        }

        groundTransform = null;
        return false;
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        throw new System.NotImplementedException();
    }

    public void AbsorbedItem(AbsorbableItemType itemType)
    {
        switch (itemType)
        {
            case AbsorbableItemType.JellyBean:
                JellyBeans++;
                Debug.Log("Jelly beans: " + JellyBeans);
                break;
            default:
                break;
        }
    }

    private void JustLanded()
    {
        footStepAudioSource.PlayNext(); //there are two feet
        footStepAudioSource.PlayNext();
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        footStepAudioSource.PlayNext();
    }

    private void UpdateGroundedPosition()
    {
        if (lastGroundedPosition == null)
        {
            lastGroundedPosition = new GroundedPositionInformation(groundTransform, Time.time, transform.position);
            return;
        }

        if (groundTransform == lastGroundedPosition.Transform) //if the current ground position is on the same transform we will just update the lastGroundedPosition
        {
            lastGroundedPosition.UpdatePosition(transform.position);
        }
        else //if we are now on another transform we should store the previous in the dictionary for previous ground positions so we can go back to it later
        {
            GroundedPositionInformation position = null;
            if (!previousGroundedPositions.ContainsKey(lastGroundedPosition.Transform))
            { //create entry if it doesn't exist
                position = new GroundedPositionInformation(lastGroundedPosition.Transform, Time.time, transform.position);
                previousGroundedPositions.Add(position.Transform, position);

                if (previousGroundedPositions.Keys.Count > 10)
                {
                    previousGroundedPositions.Remove(previousGroundedPositions.OrderBy(x => x.Value.Time).First().Key);
                }
            }
            else
            { //update entry if it does exist
                position = previousGroundedPositions[lastGroundedPosition.Transform];
                position.UpdatePosition(transform.position);
            }

            if (previousGroundedPositions.ContainsKey(groundTransform))
            {
                lastGroundedPosition = previousGroundedPositions[groundTransform]; //if we already have this in our list of previous groundpositions we should get that entry
                lastGroundedPosition.UpdatePosition(transform.position);
            }
            else
            {
                lastGroundedPosition = new GroundedPositionInformation(groundTransform, Time.time, transform.position); //since we don't have this already we will create a new
            }
        }
    }
}

public enum PlayerInputAction
{
    Attack, Jump, Move, RotateCameraRight, RotateCameraLeft, Pause
}