using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MovingCharacter
{
    public Rigidbody RigidBody;

    public Player Player;
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
    public int MagicLevel;
    public float TimeToFullSpeed = 1f;

    public float InteractCooldownTime = 0.2f;

    public MagicLevelSettingsEntry MagicSettings { get; set; }
    public bool ChargingProjectile { get { return isChargingProjectile; } }
    public int MagicProjectileId { get; set; }
    public float CurrentRunSpeed { get; set; }
    public bool ControlsEnabled { get { return _controlsEnabled; } set { if (!value) move = Vector2.zero; _controlsEnabled = value; } }
    private bool _controlsEnabled;
    public Holdable HeldItem { get; private set; }

    public CapsuleCollider Collider { get; set; }

    public Vector3 HoldPoint { get { return CalculateHoldPosition(); } }
    public PlayerNetworkCharacter PlayerNetworkCharacter { get { return playerNetworkCharacter; } }

    public override event AttackHandler OnAttack;

    public delegate void OnParentUpdatedHandler(MoveOnTrigger moveOnTriggerParent);
    public event OnParentUpdatedHandler OnParentUpdated;

    public delegate void OnLandedOnBouncyObjectHandler();
    public event OnLandedOnBouncyObjectHandler OnLandedOnBouncyObject;

    public override bool StopFootSetDefault { get { return false; } }
    public override bool? StandingStill { get { if (!playerNetworkCharacter.IsLocalPlayer) return playerNetworkCharacter.IsStandingStill; return move == Vector2.zero; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    public Dictionary<Transform, GroundedPositionInformation> PreviousGroundedPositions { get; set; }
    public GroundedPositionInformation LastGroundedPosition { get; set; }

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
    private ProjectileConfigurationEntry magicProjectileConfiguration;
    private MoveOnTrigger moveOnTriggerGround;
    private Vector3 moveOnTriggerGroundVelocity;
    private MoveBehaviour moveBehaviourGround;
    private Vector3 moveBehaviourVelocity;

    private Dictionary<Transform, MoveOnTrigger> moveOnTriggerLookup = new Dictionary<Transform, MoveOnTrigger>();
    private Dictionary<Transform, MoveBehaviour> moveBehaviourLookup = new Dictionary<Transform, MoveBehaviour>();
    private Dictionary<Transform, MoveableBlock> moveableBlockLookup = new Dictionary<Transform, MoveableBlock>();

    private bool hasDoubleJumped;
    private float lastJumpTime;
    private Collision lastCollision;
    private float rotateCamera;
    private float lastInteractTime;
    private float projectileChargeAmount;
    private bool isChargingProjectile;
    private MagicCharge currentMagicCharge;
    private float startMoveTime;
    private float currentMovementSpeedMultiplier;

    private PlayerControls boundPlayerControls;

    private List<ContactPoint> contactPoints = new List<ContactPoint>();

    private void Awake()
    {
        PreviousGroundedPositions = new Dictionary<Transform, GroundedPositionInformation>();
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

        ApplyMagicLevelSettings();

        groundedChecker.OnBecameGrounded += JustLanded;
        groundedChecker.OnNewGroundWasEntered += NewGroundWasEntered;
        Player.Health.OnDied += Died;

        CurrentRunSpeed = Speed;

        inputActions.Add(playerControls.Gameplay.Attack.id, PlayerInputAction.Attack);
        inputActions.Add(playerControls.Gameplay.Jump.id, PlayerInputAction.Jump);
        inputActions.Add(playerControls.Gameplay.Move.id, PlayerInputAction.Move);
        inputActions.Add(playerControls.Gameplay.RotateCameraLeft.id, PlayerInputAction.RotateCameraLeft);
        inputActions.Add(playerControls.Gameplay.RotateCameraRight.id, PlayerInputAction.RotateCameraRight);
        inputActions.Add(playerControls.Gameplay.Pause.id, PlayerInputAction.Pause);
        inputActions.Add(playerControls.Gameplay.RotateCameraWithMouse.id, PlayerInputAction.MouseLook);
        inputActions.Add(playerControls.Gameplay.Zoom.id, PlayerInputAction.Zoom);
        inputActions.Add(playerControls.Gameplay.SecondaryAttack.id, PlayerInputAction.SecondaryAttack);
    }

    private void Start()
    {
        if (ProjectileConfiguration.Projectiles.ContainsKey(MagicProjectileId))
        {
            magicProjectileConfiguration = ProjectileConfiguration.Projectiles[MagicProjectileId];
            Player.SetMagicProjectileId(MagicProjectileId);
        }
        else
            Debug.LogError("Missing magic projectile! Id: " + MagicProjectileId);

        Player.TurnOffRunParticles();

        Player.PlayerEars.EnableSound();
    }

    private void OnDestroy()
    {
        groundedChecker.OnBecameGrounded -= JustLanded;
        groundedChecker.OnNewGroundWasEntered -= NewGroundWasEntered;
        Player.Health.OnDied -= Died;

        if (boundPlayerControls != null)
            UnbindInputFromPlayerControls(boundPlayerControls);
    }

    public void ApplyMagicLevelSettings()
    {
        MagicLevelSettings.Load();
        MagicSettings = MagicLevelSettings.GetSettingsForLevel(MagicLevel);
    }

    public void InitializePlayerInput(PlayerConfiguration playerConfiguration, SingleTargetCamera singleTargetCamera)
    {
        Debug.Log(GameStateManager.State);

        if (GameStateManager.State == GameState.Free)
        {
            MagicLevel = MagicLevelSettings.Levels.Keys.Max();
            ApplyMagicLevelSettings();
        }

        this.playerConfiguration = playerConfiguration;
        this.singleTargetCamera = singleTargetCamera;
        Camera = singleTargetCamera.transform;

        if (!CustomNetworkManager.IsOnlineSession)
            playerConfiguration.Input.onActionTriggered += HandleAction;

        if (CustomNetworkManager.IsOnlineSession && playerNetworkCharacter.IsLocalPlayer)
        {
            PlayerControls input = new PlayerControls();

            BindInputFromPlayerControls(input);
            boundPlayerControls = input;

            input.Gameplay.Enable();
            input.Ui.Disable();
        }

        Player.PlayerEars.SetCamera(singleTargetCamera);
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
        input.Gameplay.SecondaryAttack.performed += HandleAction;

        input.Gameplay.Attack.canceled += HandleAction;
        input.Gameplay.Move.canceled += HandleAction;
        input.Gameplay.Jump.canceled += HandleAction;
        input.Gameplay.RotateCameraLeft.canceled += HandleAction;
        input.Gameplay.RotateCameraRight.canceled += HandleAction;
        input.Gameplay.Pause.canceled += HandleAction;
        input.Gameplay.Zoom.canceled += HandleAction;
        input.Gameplay.SecondaryAttack.canceled += HandleAction;
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
        input.Gameplay.SecondaryAttack.performed -= HandleAction;

        input.Gameplay.Attack.canceled -= HandleAction;
        input.Gameplay.Move.canceled -= HandleAction;
        input.Gameplay.Jump.canceled -= HandleAction;
        input.Gameplay.RotateCameraLeft.canceled -= HandleAction;
        input.Gameplay.RotateCameraRight.canceled -= HandleAction;
        input.Gameplay.Pause.canceled -= HandleAction;
        input.Gameplay.Zoom.canceled -= HandleAction;
        input.Gameplay.SecondaryAttack.canceled -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (ControlsEnabled)
        {
            if (!inputActions.ContainsKey(context.action.id))
            {
                Debug.LogWarning("Unknown action: " + context.action.name + " context: " + context.ToString());
                return;
            }

            PlayerInputAction action = inputActions[context.action.id];

            switch (action)
            {
                case PlayerInputAction.Attack:
                    if (context.performed)
                        Interact();
                    break;
                case PlayerInputAction.SecondaryAttack:
                    if (context.performed)
                    {
                        StartChargeProjectile();
                    }
                    else if (context.canceled)
                    {
                        bool didShoot = false;
                        if (isChargingProjectile && projectileChargeAmount > MagicSettings.ChargeTime)
                        {
                            ShootProjectile();
                            didShoot = true;
                        }

                        StopChargeProjectile(didShoot); //if we did shoot the charge should dissappear instantly
                    }
                    break;
                case PlayerInputAction.Jump:
                    if (context.performed)
                        Jump();
                    break;
                case PlayerInputAction.Move:
                    if (context.performed)
                    {
                        if (move == Vector2.zero)
                        {
                            startMoveTime = Time.time;
                        }

                        Move(context.ReadValue<Vector2>());

                        if (GetShouldTurnOnRunParticles(groundedChecker.GroundBlock))
                            Player.TurnOnRunParticles();
                    }
                    else
                    {
                        Move(Vector2.zero);

                        Player.TurnOffRunParticles();
                    }
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
                    Debug.LogWarning("Unknown action: " + context.action.name + " context: " + context.ToString());
                    return;
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
            if (hit.point == Vector3.zero)
                continue;

            debugHits.Add(hit.point);
            Vector3 wallDirection = new Vector3(hit.normal.z, hit.normal.y, hit.normal.x * -1).normalized;
            movement = Vector3.Dot(wallDirection, movement) * wallDirection;
        }

        foreach (ContactPoint contactPoint in contactPoints.Where(x => x.point.y > transform.position.y + 0.3f))
        {
            if (contactPoint.point == Vector3.zero)
                continue;

            debugHits.Add(contactPoint.point);

            if (groundedChecker.IsGrounded)
            {
                if (contactPoint.otherCollider != null)
                {
                    if (!moveableBlockLookup.ContainsKey(contactPoint.otherCollider.transform))
                    {
                        moveableBlockLookup.Add(contactPoint.otherCollider.transform, contactPoint.otherCollider.transform.GetComponent<MoveableBlock>());
                    }

                    MoveableBlock moveableBlock = moveableBlockLookup[contactPoint.otherCollider.transform];

                    if (moveableBlock != null)
                    {
                        Vector3 transformPositionSameHeight = new Vector3(transform.position.x, contactPoint.point.y, transform.position.z);
                        Vector3 collisionDirection = (contactPoint.point - transformPositionSameHeight).normalized;
                        if (Vector3.Angle(collisionDirection, transform.forward) < 45)
                        {
                            contactVector = new Ray(contactPoint.point, collisionDirection);
                            moveableBlock.Push(collisionDirection);
                        }
                    }
                }
            }

            if (contactPoint.otherCollider == null || contactPoint.otherCollider.attachedRigidbody == null || contactPoint.otherCollider.attachedRigidbody.isKinematic)
            {
                Vector3 collisionDirection = (contactPoint.point - transform.position).normalized;

                if (Vector3.Angle(collisionDirection, transform.forward) < 90)
                {
                    Vector3 wallDirection = new Vector3(contactPoint.normal.z, contactPoint.normal.y, contactPoint.normal.x * -1).normalized;
                    movement = Vector3.Dot(wallDirection, movement) * wallDirection;
                }
            }
        }

        Vector3 groundVelocity = Vector3.zero;

        if (IsGrounded)
        {
            if (moveOnTriggerGround != null && moveOnTriggerGround.Moving)
            {
                moveOnTriggerGroundVelocity = moveOnTriggerGround.CurrentVelocity;
                groundVelocity = moveOnTriggerGroundVelocity;
                averageVelocityKeeper.Parent = moveOnTriggerGround.AverageVelocityKeeper;
            }
            else if (moveBehaviourGround != null && moveBehaviourGround.Moving)
            {
                groundVelocity = moveBehaviourGround.CurrentMovement;
            }
            else
            {
                averageVelocityKeeper.Parent = null;
            }

            UpdateGroundedPosition();
        }
        else
        {
            transform.parent = null; //this is a bit wierd tbh, I don't know if this serves any other purpose than to show the player higher up in the hierarchy for easier access when debugging
        }

        float moveTime = Time.time - startMoveTime;
        if (moveTime < TimeToFullSpeed)
            currentMovementSpeedMultiplier = (Mathf.Sin(moveTime * Mathf.PI * (1 / TimeToFullSpeed) - (Mathf.PI / 2)) + 1f) / 2f;
        else
            currentMovementSpeedMultiplier = 1f;

        Vector3 newVelocity = movement * CurrentRunSpeed * currentMovementSpeedMultiplier;
        RigidBody.velocity = new Vector3(newVelocity.x + groundVelocity.x, RigidBody.velocity.y, newVelocity.z + groundVelocity.z);

        Vector3 newPosition = RigidBody.transform.position + movement;
        if ((newPosition - transform.position != Vector3.zero) && move != Vector2.zero) //rotate the player towards where it's going
            RigidBody.MoveRotation(Quaternion.LookRotation(movementX + movementY, Vector3.up));

        if (isChargingProjectile)
            projectileChargeAmount += Time.deltaTime / MagicSettings.ChargeTime;

        RigidBody.AddForce(-Vector3.up * Time.deltaTime * 250); //make the player fall faster because the default fall rate is to slow
    }

    private MoveOnTrigger GetMoveOnTrigger(Transform transform)
    {
        if (!moveOnTriggerLookup.ContainsKey(transform))
            moveOnTriggerLookup.Add(transform, transform.GetComponentInParent<MoveOnTrigger>());

        return moveOnTriggerLookup[transform];
    }

    private MoveBehaviour GetMoveBehaviour(Transform transform)
    {
        if (!moveBehaviourLookup.ContainsKey(transform))
            moveBehaviourLookup.Add(transform, transform.GetComponentInParent<MoveBehaviour>());

        return moveBehaviourLookup[transform];
    }

    private void FixedUpdate()
    {
        ClimbStep(0.1f, 0.6f, 0.5f, 0.1f);
    }

    public void StopChargeProjectile(bool disappearInstantly)
    {
        CurrentRunSpeed = Speed;
        projectileChargeAmount = 0;
        isChargingProjectile = false;
        Player.StopCasting(disappearInstantly);
    }

    private void StartChargeProjectile()
    {
        if (HeldItem != null || MagicSettings.Number == 0) //cannot start charing magic while holding something, and cannot charge if level 0
            return;

        if (MagicSettings.ConsumeJellyBeans && Player.JellyBeans <= 0) //can't start charging magic if no jelly beans are available
            return;

        if (isChargingProjectile)
            return;

        CurrentRunSpeed = Speed * 0.6f;
        isChargingProjectile = true;
        projectileChargeAmount = 0;
        Player.StartCasting(MagicSettings.ChargeTime, PunchHeight, 0.4f);
    }

    private void ShootProjectile()
    {
        Player.ShootProjectile(PunchHeight, MagicSettings.ProjectileLifeTime);
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

    private Ray contactVector;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 pos in debugHits)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }

        Gizmos.DrawRay(transform.position + transform.up * 0.2f, transform.forward * (0.5f + Collider.radius));
        Gizmos.DrawRay(transform.position + transform.up * 0.6f, transform.forward * (0.5f + Collider.radius));

        Gizmos.DrawRay(contactVector);
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


    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        contactPoints.Clear();
        foreach (ContactPoint contactPoint in contacts)
        {
            contactPoints.Add(contactPoint);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        contactPoints.Clear();
        foreach (ContactPoint contactPoint in contacts)
        {
            contactPoints.Add(contactPoint);
        }
    }

    private void Interact()
    {
        if (Time.time - lastInteractTime < InteractCooldownTime)
            return;

        if (isChargingProjectile) //cannot interact while charging projectile
            return;

        lastInteractTime = Time.time;

        bool shouldPunch = true;
        Vector3 pickupPosition = transform.position + (transform.up * 0.4f) + (transform.forward * 0.5f); //the center point of the spehere which we will use for pick up detection

        Collider[] colliders = Physics.OverlapSphere(pickupPosition, 0.5f);

        foreach (Collider collider in colliders) //this is for item triggers
        {
            ItemTrigger itemTrigger = collider.GetComponent<ItemTrigger>();

            if (itemTrigger != null && !itemTrigger.IsSatisfied) //there is an item trigger here
            {
                switch (itemTrigger.Properties.ItemType)
                {
                    case ItemTrigger.TriggerItemType.JellyBean:
                        if (Player.JellyBeans >= itemTrigger.Properties.Amount)
                        {
                            Player.RemoveItem(AbsorbableItemType.JellyBean, itemTrigger.Properties.Amount);
                            itemTrigger.WasSatisfied();
                        }
                        return;
                    case ItemTrigger.TriggerItemType.Coin:
                        if (Player.Coins >= itemTrigger.Properties.Amount)
                        {
                            Player.RemoveItem(AbsorbableItemType.Coin, itemTrigger.Properties.Amount);
                            itemTrigger.WasSatisfied();
                        }
                        return;
                    default:
                        return;
                }
            }
        }

        if (HeldItem == null)
        {
            Holdable holdable = null;
            foreach (Collider collider in colliders.Where(x => x.transform.tag == "Interactable")) //try to get the holdable component
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
                holdable.WasPickedUp(SpineTransform);
                HeldItem = holdable;
                CurrentRunSpeed = Speed * 0.8f;
            }
        }
        else
        {
            HeldItem.WasDropped(RigidBody.velocity, averageVelocityKeeper.Velocity);
            HeldItem = null;
            CurrentRunSpeed = Speed;
            shouldPunch = false;
        }

        if (shouldPunch)
            Punch();
    }

    private Vector3 CalculateHoldPosition()
    {
        return transform.position + transform.up * PunchHeight + transform.forward * PunchDistance * 1.1f; //1.1 is the multiplier for how far forward we should hold the item
    }

    private void Punch()
    {
        AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
        Vector3 punchPosition = transform.position + transform.forward * PunchDistance + transform.up * PunchHeight;
        punchPosition += transform.right * PunchWidth * (side == AttackSide.Right ? 1f : -1f);
        OnAttack?.Invoke(punchPosition, side);

        if (CustomNetworkManager.IsOnlineSession)
            playerNetworkCharacter.Punch(punchPosition, side);

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

                if (!CustomNetworkManager.IsOnlineSession)
                    Instantiate(DoubleJumpSmokePrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
                else
                    NetworkMethodCaller.Instance.Instantiate(6, transform.position + Vector3.up * 0.2f, Quaternion.identity); //6 is the index of the smokepoof in the network manager's list of prefabs
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
        //player movement was attacked is handeled by health
    }

    public void LandedOnBouncyObject()
    {
        hasDoubleJumped = false; //if we landed and did not bounce we should reset the player has double jumped value
        OnLandedOnBouncyObject?.Invoke();
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        if (moveOnTriggerGround != null)
        {
            moveOnTriggerGround.RemovePlayer(this);
            moveOnTriggerGround = null;
        }

        if (moveBehaviourGround != null)
        {
            moveBehaviourGround.RemovePlayer(this);
            moveBehaviourGround = null;
        }
    }

    private void NewGroundWasEntered(Block newBlock)
    {
        if (GetShouldTurnOnRunParticles(newBlock))
            TurnOnRunParticles();
        else
            TurnOffRunParticles();

        if (newBlock != null && newBlock.Instance != null)
        {
            MoveOnTrigger moveOnTrigger = GetMoveOnTrigger(newBlock.Instance.transform);
            if (moveOnTrigger != null)
            {
                if (moveOnTriggerGround != null)
                    moveOnTriggerGround.RemovePlayer(this);
                Debug.Log("Entered moveOn Trigger");
                moveOnTriggerGround = moveOnTrigger;
                moveOnTriggerGround.AddPlayer(this);
            }
            else
            {
                if (moveOnTriggerGround != null)
                {
                    moveOnTriggerGround.RemovePlayer(this);
                    moveOnTriggerGround = null;
                }
            }

            MoveBehaviour moveBehaviour = GetMoveBehaviour(newBlock.Instance.transform);
            if (moveBehaviour != null)
            {
                if (moveBehaviourGround != null)
                    moveBehaviourGround.RemovePlayer(this);
                Debug.Log("Entered moveOn Trigger");
                moveBehaviourGround = moveBehaviour;
                moveBehaviourGround.AddPlayer(this);
            }
            else
            {
                if (moveBehaviourGround != null)
                {
                    moveBehaviourGround.RemovePlayer(this);
                    moveBehaviourGround = null;
                }
            }
        }
    }

    private bool GetShouldTurnOnRunParticles(Block block)
    {
        bool turnOn = false;

        if (block != null)
        {
            if (block.Info != null && block.Info.BlockMaterial == BlockMaterial.Grass)
                turnOn = true;
        }

        return turnOn;
    }

    private void TurnOnRunParticles()
    {
        Player.TurnOnRunParticles();
    }

    private void TurnOffRunParticles()
    {
        Player.TurnOffRunParticles();
    }

    private void JustLanded()
    {
        if (!groundedChecker.GroundTransformIsBouncy)
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
        if (LastGroundedPosition == null)
        {
            LastGroundedPosition = new GroundedPositionInformation(groundedChecker.GroundTransform, Time.time, transform.position);

            if (PreviousGroundedPositions.Count == 0)
                PreviousGroundedPositions.Add(LastGroundedPosition.Transform, LastGroundedPosition);

            return;
        }

        if (groundedChecker.GroundTransform == LastGroundedPosition.Transform) //if the current ground position is on the same transform we will just update the lastGroundedPosition
        {
            LastGroundedPosition.UpdatePosition(transform.position);
        }
        else //if we are now on another transform we should store the previous in the dictionary for previous ground positions so we can go back to it later
        {
            GroundedPositionInformation position = null;
            if (!PreviousGroundedPositions.ContainsKey(LastGroundedPosition.Transform))
            { //create entry if it doesn't exist
                position = new GroundedPositionInformation(LastGroundedPosition.Transform, Time.time, transform.position);
                PreviousGroundedPositions.Add(position.Transform, position);

                if (PreviousGroundedPositions.Keys.Count > 10)
                {
                    PreviousGroundedPositions.Remove(PreviousGroundedPositions.OrderBy(x => x.Value.Time).First().Key);
                }
            }
            else
            { //update entry if it does exist
                position = PreviousGroundedPositions[LastGroundedPosition.Transform];
                position.UpdatePosition(transform.position);
            }

            if (PreviousGroundedPositions.ContainsKey(groundedChecker.GroundTransform))
            {
                LastGroundedPosition = PreviousGroundedPositions[groundedChecker.GroundTransform]; //if we already have this in our list of previous groundpositions we should get that entry
                LastGroundedPosition.UpdatePosition(transform.position);
            }
            else
            {
                LastGroundedPosition = new GroundedPositionInformation(groundedChecker.GroundTransform, Time.time, transform.position); //since we don't have this already we will create a new
            }
        }
    }
}

public enum PlayerInputAction
{
    Attack, Jump, Move, RotateCameraRight, RotateCameraLeft, Pause, MouseLook, Zoom, SecondaryAttack
}