using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MovingCharacter
{
    public float DiscoveryRadius = 20f;
    public float RoamNewTargetRange = 5f;
    public float PersonalBoundaryDistance = 0.7f;
    public NavMeshAgent Navigation;
    public UniqueSoundSource UniqueSoundSource;
    public FootStepAudioSource FootStepAudio;
    public Health Health;
    public RobotHatchController Hatches;
    public GameObject JellyBeanPrefab;
    public GameObject SmokePoofPrefab;

    public override bool StopFootSetDefault { get { return false; } }

    public override bool IsGrounded { get { return groundedChecker.IsGrounded; } }

    public override bool? StandingStill { get { if (shouldOverrideStandingStill) return standingStillOverride; else return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    public float DistanceToTargetSquared { get { return (TargetPosition - transform.position).sqrMagnitude; } }
    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; TargetWasUpdated(); } }
    private Vector3 _targetPosition;

    private Dictionary<Transform, Health> healthLookup = new Dictionary<Transform, Health>();
    private Dictionary<Transform, uint> playerNetIdLookup = new Dictionary<Transform, uint>();
    private Dictionary<uint, Transform> netIdPlayerLookup = new Dictionary<uint, Transform>();

    private GroundedChecker groundedChecker;
    private float personalBoundaryDistanceSquared;
    private bool isFacingTarget = false;
    private bool isAtTarget = false;
    private float rotationLerpTime = 0;
    private float standingStillTime = 0;
    private float lastJellyBeanSpawnTime;
    private Transform victim;
    private float setVictimTime;
    private bool previousStandingStill;
    private bool standingStillOverride;
    private bool shouldOverrideStandingStill;

    private void Awake()
    {
        groundedChecker = gameObject.GetComponent<GroundedChecker>();
        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;
    }

    private void Start()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
            shouldOverrideStandingStill = true;

        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer)
            SetIsStandingStill((bool)StandingStill);
    }

    private void Update()
    {
        if (CustomNetworkManager.HasAuthority)
            Think();

        if (!isFacingTarget)
        {
            Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), rotationLerpTime);
            rotationLerpTime += Time.deltaTime * 4 * 0.01f;

            if (Quaternion.Angle(Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), transform.rotation) <= 1 || DistanceToTargetSquared <= personalBoundaryDistanceSquared)
            {
                StartedFacingTarget();
            }
        }
    }

    private void Think()
    {
        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer && (bool)StandingStill != previousStandingStill)
        {
            SetIsStandingStill((bool)StandingStill);
        }

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
                    ThinkAboutVictim();
                }
            }
        }

        if (TargetPosition == Vector3.zero)
        {
            FindNewRoamTarget();
        }

        if (!isAtTarget && DistanceToTargetSquared < personalBoundaryDistanceSquared)
        {
            ReachedTarget();
        }

        if ((bool)StandingStill)
        {
            standingStillTime += Time.deltaTime;
        }
        else
        {
            standingStillTime = 0;
        }

        if (!isAtTarget && (standingStillTime > 2.5f || (standingStillTime > 0.4f && Navigation.path.status != NavMeshPathStatus.PathComplete)) && DistanceToTargetSquared > personalBoundaryDistanceSquared)
        {
            standingStillTime = 0;
            ReachedTarget();
        }

        previousStandingStill = (bool)StandingStill;
    }

    private void ThinkAboutVictim()
    {

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

        if (victim == null)
            SetIdle();
    }

    [ClientRpc]
    private void SetIsStandingStill(bool newValue)
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        standingStillOverride = newValue;
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
        this.CallWithDelay(SpawnJellyBean, 2);
        lastJellyBeanSpawnTime = Time.time;
        this.CallWithDelay(Hatches.Open, 1);
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
        if (CustomNetworkManager.HasAuthority)
        {
            JellyBean jellyBean = Instantiate(JellyBeanPrefab, Hatches.transform.position + Hatches.transform.forward, transform.rotation).GetComponent<JellyBean>();
            jellyBean.Rigidbody.isKinematic = false;
            jellyBean.NavMeshAgent.enabled = false;
            jellyBean.OnBecameGrounded += () =>
            {
                jellyBean.Rigidbody.isKinematic = true;
                jellyBean.NavMeshAgent.enabled = true;
                PlayerMovement player = FindClosestPlayer();
                jellyBean.SetTarget(player.Player.Health, player.transform, JellyBeanState.Chasing);
            };
        }

        Instantiate(SmokePoofPrefab, Hatches.transform.position + Hatches.transform.forward + Hatches.transform.up * 0.3f, transform.rotation);

        this.CallWithDelay(Hatches.Close, 1);
    }

    private void ReachedTarget()
    {
        isAtTarget = true;
        TakeAction();
    }

    private void TakeAction()
    {
        if (Time.time - lastJellyBeanSpawnTime > 5f)
        {
            StartSpawnJellyBean();
            this.CallWithDelay(FindNewRoamTarget, 4);
        }
        else
        {
            FindNewRoamTarget();
        }
    }

    private void TargetWasUpdated()
    {
        Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

        if (Quaternion.Angle(Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), transform.rotation) > 1 && DistanceToTargetSquared > personalBoundaryDistanceSquared)
        {
            isFacingTarget = false;
            rotationLerpTime = 0;
        }
        else
        {
            StartedFacingTarget();
        }
    }

    private void StartedFacingTarget() //the character has rotated towards the target and will now start to move towards it
    {
        isFacingTarget = true;

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
        Debug.Log("robot attacked");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(TargetPosition, 1);
    }
}
