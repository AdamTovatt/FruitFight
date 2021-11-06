using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovingCharacter
{
    public Rigidbody RigidBody;

    public GameObject DoubleJumpSmokePrefab;
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
    public float GameTimeLength = 0.2f;

    public float CurrentRunSpeed { get; set; }
    public bool ControlsEnabled { get; set; }
    public Holdable HeldItem { get; private set; }

    public CapsuleCollider Collider { get; set; }

    public int JellyBeans { get; set; }
    public int Coins { get; set; }

    public override event AttackHandler OnAttack;

    public delegate void JellyBeansUpdatedHandler(int newAmount);
    public event JellyBeansUpdatedHandler OnJellyBeansUpdated;

    public delegate void CoinsUpdatedHandler(int newAmount);
    public event CoinsUpdatedHandler OnCoinsUpdated;

    public delegate void OnPickedUpItemHandler(Holdable holdableItem);
    public event OnPickedUpItemHandler OnPickedUpItem;

    public delegate void OnDroppedItemHandler();
    public event OnDroppedItemHandler OnDroppedItem;

    public delegate void OnParentUpdatedHandler(MoveOnTrigger moveOnTriggerParent);
    public event OnParentUpdatedHandler OnParentUpdated;

    public override bool StopFootSetDefault { get { return false; } }
    public override bool? StandingStill { get { if (!playerNetworkCharacter.IsLocalPlayer) return playerNetworkCharacter.IsStandingStill; return move == Vector2.zero; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    private Transform Camera;
    private SingleTargetCamera singleTargetCamera;
    private PlayerControls playerControls;
    private Dictionary<System.Guid, PlayerInputAction> inputActions;
    private Health health;
    private AverageVelocityKeeper averageVelocityKeeper;
    private FootStepAudioSource footStepAudioSource;
    private SoundSource soundSource;
    private Vector2 move;
    private PlayerConfiguration playerConfiguration;
    private PlayerNetworkCharacter playerNetworkCharacter;
    private GroundedChecker groundedChecker;

    private Dictionary<Transform, MoveOnTrigger> moveOnTriggerLookup = new Dictionary<Transform, MoveOnTrigger>();

    private bool hasDoubleJumped;
    private float lastJumpTime;
    private Collision lastCollision;
    private float rotateCamera;

    private Dictionary<Transform, GroundedPositionInformation> previousGroundedPositions;
    private GroundedPositionInformation lastGroundedPosition;

    private List<float> previousDeathTimes;

    private PlayerControls boundPlayerControls;

    private void Awake()
    {
        previousDeathTimes = new List<float>();
        previousGroundedPositions = new Dictionary<Transform, GroundedPositionInformation>();
        ControlsEnabled = true;
        inputActions = new Dictionary<System.Guid, PlayerInputAction>();
        playerControls = new PlayerControls();
        RigidBody = gameObject.GetComponent<Rigidbody>();
        Collider = gameObject.GetComponent<CapsuleCollider>();
        health = gameObject.GetComponent<Health>();
        averageVelocityKeeper = gameObject.GetComponent<AverageVelocityKeeper>();
        footStepAudioSource = gameObject.GetComponent<FootStepAudioSource>();
        soundSource = gameObject.GetComponent<SoundSource>();
        playerNetworkCharacter = gameObject.GetComponent<PlayerNetworkCharacter>();
        groundedChecker = gameObject.GetComponent<GroundedChecker>();

        groundedChecker.OnBecameGrounded += JustLanded;

        CurrentRunSpeed = Speed;

        inputActions.Add(playerControls.Gameplay.Attack.id, PlayerInputAction.Attack);
        inputActions.Add(playerControls.Gameplay.Jump.id, PlayerInputAction.Jump);
        inputActions.Add(playerControls.Gameplay.Move.id, PlayerInputAction.Move);
        inputActions.Add(playerControls.Gameplay.RotateCameraLeft.id, PlayerInputAction.RotateCameraLeft);
        inputActions.Add(playerControls.Gameplay.RotateCameraRight.id, PlayerInputAction.RotateCameraRight);
        inputActions.Add(playerControls.Gameplay.Pause.id, PlayerInputAction.Pause);
        inputActions.Add(playerControls.Gameplay.RotateCameraWithMouse.id, PlayerInputAction.MouseLook);
        inputActions.Add(playerControls.Gameplay.Zoom.id, PlayerInputAction.Zoom);
    }

    private void Start()
    {
        health.OnDied += OnDied;
        OnJellyBeansUpdated?.Invoke(JellyBeans);
        OnCoinsUpdated?.Invoke(Coins);
    }

    private void OnDestroy()
    {
        health.OnDied -= OnDied;
        groundedChecker.OnBecameGrounded -= JustLanded;

        if (boundPlayerControls != null)
            UnbindInputFromPlayerControls(boundPlayerControls);
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

    public void InitializePlayerInput(PlayerConfiguration playerConfiguration, SingleTargetCamera singleTargetCamera)
    {
        this.playerConfiguration = playerConfiguration;
        this.singleTargetCamera = singleTargetCamera;
        Camera = singleTargetCamera.transform;

        playerConfiguration.Input.onActionTriggered += HandleAction;

        if (playerConfiguration.Input.currentControlScheme == "Keyboard")
        {
            PlayerControls input = new PlayerControls();
            input.Gameplay.Attack.performed += CheckForMouseInput;
            input.Gameplay.RotateCameraWithMouse.performed += CheckForMouseInput;
            input.Gameplay.RotateCameraWithMouse.canceled += MouseLookCancelled;
            input.Gameplay.Enable();
        }

        if (CustomNetworkManager.IsOnlineSession)
        {
            PlayerControls input = new PlayerControls();

            BindInputFromPlayerControls(input);
            boundPlayerControls = input;

            input.Gameplay.Enable();
        }
    }

    private void BindInputFromPlayerControls(PlayerControls input)
    {
        input.Gameplay.Attack.performed += HandleAction;
        input.Gameplay.Move.performed += HandleAction;
        input.Gameplay.Jump.performed += HandleAction;
        input.Gameplay.RotateCameraLeft.performed += HandleAction;
        input.Gameplay.RotateCameraRight.performed += HandleAction;
        input.Gameplay.Pause.performed += HandleAction;
        input.Gameplay.Zoom.performed += HandleAction;

        input.Gameplay.Attack.canceled += HandleAction;
        input.Gameplay.Move.canceled += HandleAction;
        input.Gameplay.Jump.canceled += HandleAction;
        input.Gameplay.RotateCameraLeft.canceled += HandleAction;
        input.Gameplay.RotateCameraRight.canceled += HandleAction;
        input.Gameplay.Pause.canceled += HandleAction;
        input.Gameplay.Zoom.canceled += HandleAction;
    }

    private void UnbindInputFromPlayerControls(PlayerControls input)
    {
        input.Gameplay.Attack.performed -= HandleAction;
        input.Gameplay.Move.performed -= HandleAction;
        input.Gameplay.Jump.performed -= HandleAction;
        input.Gameplay.RotateCameraLeft.performed -= HandleAction;
        input.Gameplay.RotateCameraRight.performed -= HandleAction;
        input.Gameplay.Pause.performed -= HandleAction;
        input.Gameplay.Zoom.performed -= HandleAction;

        input.Gameplay.Attack.canceled -= HandleAction;
        input.Gameplay.Move.canceled -= HandleAction;
        input.Gameplay.Jump.canceled -= HandleAction;
        input.Gameplay.RotateCameraLeft.canceled -= HandleAction;
        input.Gameplay.RotateCameraRight.canceled -= HandleAction;
        input.Gameplay.Pause.canceled -= HandleAction;
        input.Gameplay.Zoom.canceled -= HandleAction;
    }

    private void MouseLookCancelled(InputAction.CallbackContext context)
    {
        rotateCamera = 0;
    }

    private void CheckForMouseInput(InputAction.CallbackContext context)
    {
        if (context.control.ToString().ToLower().Contains("mouse"))
        {
            HandleAction(context);
        }
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
                case PlayerInputAction.MouseLook:
                    rotateCamera = Mathf.Clamp(context.ReadValue<Vector2>().x * -2f, -15, 15);
                    break;
                case PlayerInputAction.Zoom:
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
            if (!CustomNetworkManager.IsOnlineSession)
            {
                singleTargetCamera = GameManager.Instance.CameraManager.Cameras.Where(x => x.Input == playerConfiguration.Input).FirstOrDefault();
                Camera = singleTargetCamera.transform;
            }
            else
            {
                if (playerNetworkCharacter.Camera != null)
                    Camera = playerNetworkCharacter.Camera.transform;
                else
                    Debug.LogWarning("No camera in playerNetworkCharacter");
                return;
            }
        }

        Vector3 cameraForward = Camera.forward;
        Vector3 cameraRight = Camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementX = cameraRight * move.x;
        Vector3 movementY = cameraForward * move.y;

        Vector3 movement = (movementX + movementY).normalized;

        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(forwardRay, 0.4f, 0.4f, ~3);

        debugHits.Clear();

        foreach (RaycastHit hit in hits.Where(x => x.transform.position.y > transform.position.y + 0.3f))
        {
            debugHits.Add(hit.point);
            Vector3 wallDirection = new Vector3(hit.normal.z, hit.normal.y, hit.normal.x * -1).normalized;
            movement = Vector3.Dot(wallDirection, movement) * wallDirection;
        }

        Vector3 groundVelocity = Vector3.zero;

        if (IsGrounded)
        {
            if (!moveOnTriggerLookup.ContainsKey(groundedChecker.GroundTransform))
                moveOnTriggerLookup.Add(groundedChecker.GroundTransform, groundedChecker.GroundTransform.GetComponentInParent<MoveOnTrigger>());

            MoveOnTrigger moveOnTrigger = moveOnTriggerLookup[groundedChecker.GroundTransform];

            if (moveOnTrigger != null && moveOnTrigger.Moving)
            {
                groundVelocity = moveOnTrigger.CurrentVelocity * 8.2f; //I don't know why but 8.2 seems to be about the right value to multiply with to avoid slippage on moving platforms
                averageVelocityKeeper.Parent = moveOnTrigger.AverageVelocityKeeper;
            }
            else
            {
                UpdateGroundedPosition();
                averageVelocityKeeper.Parent = null;
            }
        }
        else
        {
            transform.parent = null; //this is a bit wierd tbh, I don't know if this serves any other purpose than to show the player higher up in the hierarchy for easier access when debugging
        }

        Vector3 newVelocity = movement * CurrentRunSpeed;
        RigidBody.velocity = new Vector3(newVelocity.x + groundVelocity.x, RigidBody.velocity.y, newVelocity.z + groundVelocity.z);

        Vector3 newPosition = RigidBody.transform.position + movement;
        if ((newPosition - transform.position != Vector3.zero) && move != Vector2.zero) //rotate the player towards where it's going
            RigidBody.MoveRotation(Quaternion.LookRotation(movementX + movementY, Vector3.up));

        RigidBody.AddForce(-Vector3.up * Time.deltaTime * 250); //make the player fall faster because the default fall rate is to slow
    }

    private void FixedUpdate()
    {
        ClimbStep(0.1f, 0.6f, 0.5f, 0.1f);
    }

    private void ClimbStep(float ray1Height, float ray2Height, float rayLenght, float stepHeight)
    {
        if (IsGrounded && move.sqrMagnitude > 0)
        {
            Transform lowerHitTransform = null;

            Ray lowerRay1 = new Ray(transform.position + transform.up * ray1Height, transform.forward);
            if (Physics.Raycast(lowerRay1, out RaycastHit hit, rayLenght + Collider.radius))
            {
                if (hit.transform.tag == "Ground")
                    lowerHitTransform = hit.transform;

            }
            else
            {
                Ray lowerRay2 = new Ray(transform.position + transform.up * (ray1Height + 0.2f), transform.forward);
                if (Physics.Raycast(lowerRay2, out RaycastHit hit2, rayLenght + Collider.radius))
                {
                    if (hit2.transform.tag == "Ground")
                        lowerHitTransform = hit.transform;

                }
            }

            if (lowerHitTransform != null && lowerHitTransform != groundedChecker.GroundTransform)
            {
                Ray ray2 = new Ray(transform.position + transform.up * ray2Height, transform.forward);
                if (!Physics.Raycast(ray2, out RaycastHit upperHit, rayLenght + Collider.radius))
                {
                    RigidBody.position += transform.up * stepHeight;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 pos in debugHits)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }

        Gizmos.DrawRay(transform.position + transform.up * 0.2f, transform.forward * (0.5f + Collider.radius));
        Gizmos.DrawRay(transform.position + transform.up * 0.6f, transform.forward * (0.5f + Collider.radius));
    }

    private void Move(Vector2 moveValue)
    {
        if (moveValue.magnitude > MovementInputCutoff)
        {
            move = moveValue;

            if (CustomNetworkManager.IsOnlineSession)
                playerNetworkCharacter.SetStandingStill(false);
        }
        else
        {
            move = Vector2.zero;

            if (CustomNetworkManager.IsOnlineSession)
                playerNetworkCharacter.SetStandingStill(true);
        }
    }

    private void Interact()
    {
        bool shouldPunch = true;

        if (HeldItem == null)
        {
            Vector3 pickupPosition = transform.position + (transform.up * 0.4f) + (transform.forward * 0.5f); //the center point of the spehere which we will use for pick up detection

            Holdable holdable = null;
            foreach (Collider collider in Physics.OverlapSphere(pickupPosition, 0.5f).Where(x => x.transform.tag == "Interactable")) //try to get the holdable component
            {
                holdable = collider.transform.GetComponent<Holdable>();

                if (holdable == null)
                {
                    HoldableDelegate holdableDelegate = collider.transform.GetComponentInParent<HoldableDelegate>();
                    if (holdableDelegate != null)
                        holdable = holdableDelegate.ContainedHoldable;
                }

                if (holdable != null)
                    break;
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

        bool didHit = false;
        List<Transform> checkedTransforms = new List<Transform>();
        foreach (Collider hit in Physics.OverlapSphere(PunchSphereTransform.position, PunchSphereRadius))
        {
            if (hit.transform != transform)
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
        if (Time.time - lastJumpTime > GameTimeLength + 0.1f && groundedChecker.IsGroundedWithGameTime)
        {
            PerformJump();
        }
        else
        {
            if (!hasDoubleJumped && Time.time - lastJumpTime > 0.2f)
            {
                PerformJump();
                hasDoubleJumped = true;
                Instantiate(DoubleJumpSmokePrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
            }
        }
    }

    private void PerformJump()
    {
        RigidBody.velocity = new Vector3(RigidBody.velocity.x, JumpStrength, RigidBody.velocity.z);
        lastJumpTime = Time.time;

        StepWasTaken(transform.position); //two sounds effects because two feet
        StepWasTaken(transform.position);
    }

    private void OnEnable()
    {
        ControlsEnabled = true;
    }

    private void OnDisable()
    {
        ControlsEnabled = false;
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
                OnJellyBeansUpdated?.Invoke(JellyBeans);
                break;
            case AbsorbableItemType.Coin:
                Coins++;
                OnCoinsUpdated?.Invoke(Coins);
                break;
            default:
                break;
        }
    }

    private void JustLanded()
    {
        hasDoubleJumped = false;

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
            lastGroundedPosition = new GroundedPositionInformation(groundedChecker.GroundTransform, Time.time, transform.position);
            return;
        }

        if (groundedChecker.GroundTransform == lastGroundedPosition.Transform) //if the current ground position is on the same transform we will just update the lastGroundedPosition
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

            if (previousGroundedPositions.ContainsKey(groundedChecker.GroundTransform))
            {
                lastGroundedPosition = previousGroundedPositions[groundedChecker.GroundTransform]; //if we already have this in our list of previous groundpositions we should get that entry
                lastGroundedPosition.UpdatePosition(transform.position);
            }
            else
            {
                lastGroundedPosition = new GroundedPositionInformation(groundedChecker.GroundTransform, Time.time, transform.position); //since we don't have this already we will create a new
            }
        }
    }
}

public enum PlayerInputAction
{
    Attack, Jump, Move, RotateCameraRight, RotateCameraLeft, Pause, MouseLook, Zoom
}