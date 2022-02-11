using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MovingCharacter
{
    public float DiscoveryRadius = 20f;
    public float RoamNewTargetRange = 5f;
    public float CloseRange = 2f;
    public float PunchReachSquared = 3.4f;
    public float PersonalBoundaryDistance = 0.7f;
    public float PunchDamage = 30f;
    public float TimeBetweenPunches = 2f;
    public NavMeshAgent Navigation;
    public UniqueSoundSource UniqueSoundSource;
    public FootStepAudioSource FootStepAudio;
    public Health Health;
    public RobotHatchController Hatches;
    public GameObject JellyBeanPrefab;
    public GameObject SmokePoofPrefab;
    public GameObject SmallSparkPrefab;
    public Transform RightHandPosition;
    public Transform LeftHandPosition;
    public GameObject BodySparks;
    public GameObject ElectricCharge;
    public GameObject ElectricProjectile;
    public Transform ElectricShootPoint;
    public float EnragedSwitchValue = 0.5f;

    [SyncVar]
    public int ServerStandingStill;

    public override bool StopFootSetDefault { get { return false; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    public override bool? StandingStill { get { if (shouldOverrideStandingStill) return ServerStandingStill.ToNullableBool(); else return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    public Vector3 TargetLocationSameHeight { get { return new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z); } }
    public bool IsFacingTarget { get { return Quaternion.Angle(Quaternion.LookRotation(TargetLocationSameHeight - transform.position, Vector3.up), transform.rotation) <= 1 || DistanceToTargetSquared <= personalBoundaryDistanceSquared; } }
    public float DistanceToTargetSquared { get { return (TargetPosition - transform.position).sqrMagnitude; } }
    public float DistanceToVictimSquared { get { if (victim == null) return -1; return (victim.position - transform.position).sqrMagnitude; } }
    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; TargetWasUpdated(); } }
    private Vector3 _targetPosition;

    public delegate void StartedFacingTargetHandler();
    public event StartedFacingTargetHandler OnStartedFacingTarget;

    private Dictionary<Transform, Health> healthLookup = new Dictionary<Transform, Health>();
    private Dictionary<Transform, uint> playerNetIdLookup = new Dictionary<Transform, uint>();
    private Dictionary<uint, Transform> netIdPlayerLookup = new Dictionary<uint, Transform>();

    private bool victimIsCloseRange { get { if (victim == null) return false; return DistanceToVictimSquared < CloseRange * CloseRange; } }
    private Vector3 horizontalToVictim { get { Vector3 toVictim = victim.transform.position - transform.position; toVictim.y = 0; return toVictim; } }

    private Shake[] shakingParts;

    private GroundedChecker groundedChecker;
    private float personalBoundaryDistanceSquared;
    private bool doneRotatingTowardsTarget = false;
    private bool isAtTarget = false;
    private float rotationLerpTime = 0;
    private float standingStillTime = 0;
    private float lastJellyBeanSpawnTime;
    private Transform victim;
    private float setVictimTime;
    private bool shouldOverrideStandingStill;
    private float lastPunchTime;
    private float closeRangeRepositionTime;
    private AttackSide lastAttackSide;
    private RobotState state = RobotState.Unspecified;
    private int spawnedJellyBeans = 5;
    private float lastStartNavigationTime;
    private float chargeTime;
    private bool isDisplayingHealth;
    private bool enraged;
    private MagicCharge currentMagicCharge;
    private bool startedShooting;

    private void Awake()
    {
        groundedChecker = gameObject.GetComponent<GroundedChecker>();
        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;

        shakingParts = gameObject.GetComponentsInChildren<Shake>();

        Health.OnHealthUpdated += HealthUpdated;
        BodySparks.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
        {
            Navigation.enabled = false;
            shouldOverrideStandingStill = true;
        }
    }

    private void Update()
    {
        if (CustomNetworkManager.HasAuthority)
        {
            UpdateState();
            Think();
        }

        if (!doneRotatingTowardsTarget)
        {
            Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), rotationLerpTime);
            rotationLerpTime += Time.deltaTime * 4 * 0.01f;

            if (IsFacingTarget)
            {
                doneRotatingTowardsTarget = true;
                InvokeStartedFacingTarget();
            }
        }

        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.HasAuthority)
            ServerStandingStill = StandingStill.ToInt();
    }

    private void UpdateState()
    {
        if ((victim == null || Time.time - setVictimTime > 2) && GameManager.Instance != null)
        {
            PlayerMovement player = FindClosestPlayer();
            if (player != null)
            {
                Health health = GetHealthForTransform(player.transform);

                if (health != null && !health.IsDead && health.CanDie)
                {
                    health.OnDied += VictimDied;
                    SetNewVictim(player);
                }
            }
        }

        if (victimIsCloseRange)
        {
            if (state != RobotState.CloseRange)
            {
                state = RobotState.CloseRange;

                if (!IsFacingTarget)
                    StartRotateTowardsTarget();

                TargetPosition = transform.position;
                StartNavigationTowardsTarget();
                isAtTarget = false;

                Debug.Log("Entered close range");
                return;
            }
        }
        else
        {
            if (state == RobotState.CloseRange)
            {
                Debug.Log("Exited close range");
                if (Time.time - chargeTime < 3.5f)
                {
                    state = RobotState.Charging;
                }
                else
                {
                    if (enraged)
                        state = RobotState.Shooting;
                    else
                        state = RobotState.Unspecified;
                }

                isAtTarget = false;
            }
        }

        if (state == RobotState.Unspecified)
        {
            state = RobotState.Moving;
        }
    }

    private void Think()
    {
        switch (state)
        {
            case RobotState.Moving:
                if (!isAtTarget && DistanceToTargetSquared < personalBoundaryDistanceSquared)
                {
                    ReachedTarget();
                }

                if (!isAtTarget && (standingStillTime > 2.5f || (standingStillTime > 0.4f && Navigation.path.status != NavMeshPathStatus.PathComplete)) && DistanceToTargetSquared > personalBoundaryDistanceSquared)
                {
                    standingStillTime = 0;
                    ReachedTarget();
                }

                if (isAtTarget && standingStillTime > 1f)
                {
                    ReachedTarget();
                }
                break;
            case RobotState.CloseRange:
                if (victim != null)
                {
                    if (horizontalToVictim.sqrMagnitude > PunchReachSquared * 0.8f)
                    {
                        if (Time.time - closeRangeRepositionTime > 0.2f)
                        {
                            SetNewTarget(victim.position);
                            StartNavigationTowardsTarget();
                            closeRangeRepositionTime = Time.time;
                        }
                    }
                    else
                    {
                        SetNewTarget(transform.position);
                    }

                    if (IsFacingPosition(victim.transform.position, 10f) || horizontalToVictim.sqrMagnitude < 1)
                    {
                        if (Time.time - lastPunchTime > TimeBetweenPunches)
                        {
                            AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
                            lastAttackSide = side;
                            Punch(side);
                            lastPunchTime = Time.time;
                            this.CallWithDelay(ApplyPlayerPunch, 0.2f);
                        }
                    }
                    else
                    {
                        doneRotatingTowardsTarget = true;
                        RotateTowardsPosition(victim.transform.position);
                    }
                }
                break;
            case RobotState.Charging:
                if (victim != null)
                {
                    TargetPosition = victim.transform.position;
                    RotateTowardsPosition(TargetPosition);

                    if (Time.time - lastStartNavigationTime > 0.2f)
                    {
                        lastStartNavigationTime = Time.time;
                        StartNavigationTowardsTarget();
                    }
                }
                else
                {
                    state = RobotState.Unspecified;
                }
                break;
            case RobotState.Unspecified:
                if (Time.time - chargeTime < 7f && victim != null)
                    state = RobotState.Charging;
                else
                    state = RobotState.Moving;
                break;
            case RobotState.Shooting:
                if (!startedShooting)
                    StartShooting();
                break;
            default:
                break;
        }

        if (TargetPosition == Vector3.zero)
        {
            FindNewRoamTarget();
        }

        if ((bool)StandingStill)
        {
            standingStillTime += Time.deltaTime;
        }
        else
        {
            standingStillTime = 0;
        }
    }

    private void Punch(AttackSide side)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcPunch((int)side);
                PerformPunch(side);
            }
        }
        else
        {
            PerformPunch(side);
        }
    }

    [ClientRpc]
    private void RpcPunch(int side)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformPunch((AttackSide)side);
    }


    private void PerformPunch(AttackSide side)
    {
        OnAttack?.Invoke(victim.position + Vector3.up * 0.4f, side);
    }

    private void ApplyPlayerPunch()
    {
        if (DistanceToVictimSquared < PunchReachSquared && IsFacingPosition(victim.transform.position, 15f) || horizontalToVictim.sqrMagnitude < 1)
        {
            Vector3 handPosition = lastAttackSide == AttackSide.Left ? LeftHandPosition.position : RightHandPosition.position;
            CreateSpark(handPosition);

            Health health = GetHealthForTransform(victim.transform);
            health.TakeDamage(PunchDamage);

            Vector3 explosionOrigin = health.transform.position + (transform.position - health.transform.position).normalized;// + Vector3.up * -0.1f;
            explosionOrigin.y = health.transform.position.y;
            health.GetComponent<PlayerMovement>().RigidBody.AddExplosionForce(10f * PunchDamage, explosionOrigin, 2f);

            lastPunchTime += 0.2f; //if the punch landed we don't want to punch again as soon
        }
        else
        {
            lastPunchTime -= 1.2f; //if the punch didn't land we want to punch again soon
        }
    }

    private void CreateSpark(Vector3 position)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcCreateSpark(position);
                PerformCreateSpark(position);
            }
            else
            {
                Debug.LogError("Client tried to create spark, this should not be able to happen");
            }
        }
        else
        {
            PerformCreateSpark(position);
        }
    }

    [ClientRpc]
    private void RpcCreateSpark(Vector3 position)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformCreateSpark(position);
    }

    private void PerformCreateSpark(Vector3 position)
    {
        Instantiate(SmallSparkPrefab, position, Quaternion.identity);
    }

    private void ThinkAboutVictim()
    {

    }

    private bool IsFacingPosition(Vector3 position, float minAngle)
    {
        return Quaternion.Angle(Quaternion.LookRotation(position - transform.position, Vector3.up), transform.rotation) <= minAngle;
    }

    private void RotateTowardsPosition(Vector3 position)
    {
        Vector3 newForward = Vector3.RotateTowards(transform.forward, position - transform.position, 5f * Time.deltaTime, 0);
        newForward.y = transform.forward.y;
        transform.forward = newForward;
    }

    private void VictimDied(Health sender, CauseOfDeath causeOfDeath)
    {
        SetNewVictim(null);
    }

    private void SetIdle()
    {

    }

    private void SetNewVictim(PlayerMovement player)
    {
        setVictimTime = Time.time;

        if (player != null)
        {
            if (!playerNetIdLookup.ContainsKey(player.transform))
                playerNetIdLookup.Add(player.transform, player.transform.GetComponent<NetworkIdentity>().netId);
        }

        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer)
            RpcSetVictim(player == null ? 0 : playerNetIdLookup[player.transform]);

        PerformSetVictim(player == null ? null : player.transform);
    }

    [ClientRpc]
    private void RpcSetVictim(uint netId)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        if (netId != 0)
        {
            if (!netIdPlayerLookup.ContainsKey(netId))
                netIdPlayerLookup.Add(netId, NetworkClient.spawned[netId].transform);
        }

        PerformSetVictim(netId == 0 ? null : netIdPlayerLookup[netId]);
    }

    private void PerformSetVictim(Transform victim)
    {
        if (CustomNetworkManager.HasAuthority && this.victim != null) //remove old victim event listener
            healthLookup[this.victim].OnDied -= VictimDied;
        if (CustomNetworkManager.HasAuthority && victim != null) //add new victim event listener
            healthLookup[victim].OnDied += VictimDied;

        this.victim = victim;

        if (!isDisplayingHealth && victim != null)
        {
            GameUi.Instance.DisplayHealth(Health, Health.HpPerHeart);
            isDisplayingHealth = true;
        }

        if (victim == null && isDisplayingHealth)
        {
            GameUi.Instance.StopDisplayingHealth();
            isDisplayingHealth = false;
        }

        if (victim == null)
            SetIdle();
    }

    private Health GetHealthForTransform(Transform transform)
    {
        if (!healthLookup.ContainsKey(transform))
            healthLookup.Add(transform, transform.gameObject.GetComponent<Health>());

        return healthLookup[transform];
    }

    private void StartSpawnJellyBean()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcStartSpawnJellyBean();
                PerformStartSpawnJellyBean();
            }
            else
            {
                return;
            }
        }
        else
        {
            PerformStartSpawnJellyBean();
        }
    }

    private void PerformStartSpawnJellyBean()
    {
        state = RobotState.Spawning;
        this.CallWithDelay(SpawnJellyBean, 1.4f);
        lastJellyBeanSpawnTime = Time.time;
        this.CallWithDelay(Hatches.Open, 0.2f);
        spawnedJellyBeans++;
    }

    [ClientRpc]
    private void RpcStartSpawnJellyBean()
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;
        PerformStartSpawnJellyBean();
    }

    private void SpawnJellyBean()
    {
        if (Time.time - lastJellyBeanSpawnTime < 0.2f)
        {
            return;
        }

        if (CustomNetworkManager.HasAuthority)
        {
            JellyBean jellyBean = Instantiate(JellyBeanPrefab, Hatches.transform.position + Hatches.transform.forward, transform.rotation).GetComponent<JellyBean>();

            if (CustomNetworkManager.IsOnlineSession)
                NetworkServer.Spawn(jellyBean.gameObject, PlayerNetworkCharacter.LocalPlayer.connectionToServer);

            jellyBean.SetWasSummoned();

            jellyBean.OnBecameGrounded += () =>
            {
                jellyBean.SetSummoningCompleted();
                PlayerMovement player = FindClosestPlayer();
                if (player != null)
                    jellyBean.SetTarget(player.Player.Health, player.transform, JellyBeanState.Chasing);
            };
        }

        Instantiate(SmokePoofPrefab, Hatches.transform.position + Hatches.transform.forward + Hatches.transform.up * 0.3f, transform.rotation);

        this.CallWithDelay(Hatches.Close, 0.5f);
        state = RobotState.Moving;
        lastJellyBeanSpawnTime = Time.time;
    }

    private void ReachedTarget()
    {
        if (isAtTarget)
            return;

        isAtTarget = true;
        TakeAction();
    }

    private void TakeAction()
    {
        Debug.Log("Take action: " + state.ToString());
        if (state != RobotState.CloseRange)
        {
            if (spawnedJellyBeans < 3)
            {
                if (Time.time - lastJellyBeanSpawnTime > 1f)
                {
                    if (victim != null)
                    {
                        TargetPosition = victim.transform.position;
                        StartRotateTowardsTarget();
                        OnStartedFacingTarget += () =>
                        {
                            StartSpawnJellyBean();
                            this.CallWithDelay(FindNewRoamTarget, 4);
                        };
                    }
                    else
                    {
                        StartSpawnJellyBean();
                        this.CallWithDelay(FindNewRoamTarget, 4);
                    }
                }
                else
                {
                    FindNewRoamTarget();
                }
            }
            else
            {
                if (victim != null)
                {
                    if (state != RobotState.Charging)
                    {
                        state = RobotState.Charging;
                        chargeTime = Time.time;
                        TargetPosition = victim.transform.position;
                        StartRotateTowardsTarget();
                        OnStartedFacingTarget += () =>
                        {
                            StartNavigationTowardsTarget();
                        };
                    }
                }
                else
                {
                    FindNewRoamTarget();
                }

                spawnedJellyBeans = 0;
            }
        }
    }

    private void InvokeStartedFacingTarget()
    {
        OnStartedFacingTarget?.Invoke();
        OnStartedFacingTarget = null;
    }

    private void TargetWasUpdated()
    {
        if (!IsFacingTarget)
        {
            StartRotateTowardsTarget();
        }
        else
        {
            InvokeStartedFacingTarget();
        }
    }

    private void StartRotateTowardsTarget()
    {
        doneRotatingTowardsTarget = false;
        rotationLerpTime = 0;
    }

    private void StartNavigationTowardsTarget()
    {
        if (Navigation.isActiveAndEnabled)
            Navigation.SetDestination(TargetPosition);
    }

    private void FindNewRoamTarget()
    {
        Vector3? newPosition = GetRandomPositionWithinDistance(RoamNewTargetRange);

        if (newPosition != null)
        {
            SetNewTarget((Vector3)newPosition);
            isAtTarget = false;
        }
    }

    private void SetNewTarget(Vector3 position)
    {
        TargetPosition = position;
        OnStartedFacingTarget += StartNavigationTowardsTarget;
    }

    private void StartShooting()
    {
        startedShooting = true;

        if (currentMagicCharge != null)
            Destroy(currentMagicCharge.gameObject);

        currentMagicCharge = Instantiate(ElectricCharge, ElectricShootPoint.position, Quaternion.identity, ElectricShootPoint).GetComponent<MagicCharge>();
        currentMagicCharge.Initialize(1f);

        this.CallWithDelay(FireShot, 2f);
    }

    private void FireShot()
    {
        Vector3 victimDirection = victim == null ? transform.forward : ((victim.transform.position + Vector3.up * 0.5f) - ElectricShootPoint.transform.position).normalized;
        Vector3 shootPosition = ElectricShootPoint.transform.position + victimDirection * 2;

        Destroy(currentMagicCharge.gameObject);
        MagicProjectile projectile = Instantiate(ElectricProjectile, shootPosition, Quaternion.identity).GetComponent<MagicProjectile>();
        projectile.Shoot(shootPosition, ElectricShootPoint.transform, victimDirection, ElectricShootPoint.transform.position, 4f, true);

        state = RobotState.Unspecified;
        startedShooting = false;
    }

    private Vector3? GetRandomPositionWithinDistance(float distance, int attempt = 0, Vector3 offset = default(Vector3))
    {
        bool foundValue = false;

        Vector3 randomPosition = Random.insideUnitSphere * distance;
        randomPosition.y = transform.position.y;

        Vector3? result = transform.position + randomPosition;
        if (offset != default(Vector3))
            result += offset;

        result += Vector3.up * 0.5f;

        Ray groundRay = new Ray((Vector3)result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            if (groundHit.transform.tag == "Ground")
            {
                foundValue = true;
                result = groundHit.point;
            }
        }

        if (!foundValue)
        {
            if (attempt <= 10)
                result = GetRandomPositionWithinDistance(distance, attempt + 1); //try again
            else
                result = null; //give up
        }

        return result;
    }

    private PlayerMovement FindClosestPlayer()
    {
        PlayerMovement closestPlayer = null;
        float closestPlayerDistance = 100000;

        foreach (PlayerMovement player in GameManager.Instance.PlayerCharacters)
        {
            if (player != null)
            {
                float distanceSquared = (player.transform.position - transform.position).sqrMagnitude;
                if (distanceSquared < DiscoveryRadius * DiscoveryRadius)
                {
                    if (closestPlayerDistance > distanceSquared)
                    {
                        closestPlayer = player;
                        closestPlayerDistance = distanceSquared;
                    }
                }
            }
        }

        return closestPlayer;
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        if (UniqueSoundSource.Active)
        {
            FootStepAudio.PlayNext();
        }
        else
        {
            Debug.Log("Unique sound not active");
        }
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {

    }

    private void HealthUpdated()
    {
        if (Health.CurrentHealth < Health.StartHealth * EnragedSwitchValue)
        {
            foreach (Shake part in shakingParts)
            {
                part.StartShaking();
            }
            BodySparks.gameObject.SetActive(true);
            enraged = true;
            gameObject.GetComponent<SoundSource>().Play("enraged");
        }
    }
}

public enum RobotState
{
    Moving, CloseRange, Spawning, Unspecified, Charging, Shooting
}
