using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HardCandy : MovingCharacter
{
    public NavMeshAgent Navigation;

    public float ChargeDamage = 10f;
    public float PersonalBoundaryDistance = 0.7f;
    public float RoamNewTargetRange = 5f;
    public float DiscoveryRadius = 5f;
    public List<HardCandySpeedSettings> SpeedSettings;

    public float DistanceToGround = 0.1f;

    public UniqueSoundSource UniqueSoundSource;
    public FootStepAudioSource FootStepAudio;
    public SoundSource Sound;
    public Health Health;

    public HardCandyState CurrentState = HardCandyState.Roaming;
    public float DistanceToTargetSquared { get { return (TargetPosition - transform.position).sqrMagnitude; } }
    public float CurrentMaxSpeed { get; set; }
    public Vector3 TargetPosition { get { return _targetPosition; } set { _targetPosition = value; TargetWasUpdated(); } }
    private Vector3 _targetPosition;

    public override bool StopFootSetDefault { get { return false; } }

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

    public override bool? StandingStill { get { if (shouldOverrideStandingStill) return standingStillOverride; else return Navigation.velocity.sqrMagnitude == 0 || Navigation.isStopped; } }

    public override event AttackHandler OnAttack;

    private Dictionary<HardCandyState, HardCandySpeedSettings> speedLookup = new Dictionary<HardCandyState, HardCandySpeedSettings>();
    private Dictionary<Transform, Health> healthLookup = new Dictionary<Transform, Health>();
    private Dictionary<Transform, uint> playerNetIdLookup = new Dictionary<Transform, uint>();
    private Dictionary<uint, Transform> netIdPlayerLookup = new Dictionary<uint, Transform>();

    private bool shouldOverrideStandingStill;
    private bool standingStillOverride;
    private bool previousStandingStill;
    private bool isFacingTarget = false;
    private float rotationLerpTime = 0;
    private float personalBoundaryDistanceSquared;
    private float standingStillTime;
    private Transform victim;
    private float setVictimTime;
    private float thinkAboutVictimTime;
    private bool hasRedirectedCharge;
    private float idleTime;
    private float idleGoalTime;

    private void Awake()
    {
        foreach (HardCandySpeedSettings speedSetting in SpeedSettings)
        {
            speedLookup.Add(speedSetting.State, speedSetting);
        }

        BindEvents();
        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;
    }

    private void Start()
    {
        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
            shouldOverrideStandingStill = true;

        if (CustomNetworkManager.HasAuthority)
            FindNewRoamTarget();
    }

    private void Update()
    {
        Navigation.speed = speedLookup[CurrentState].RunSpeed;

        if (CustomNetworkManager.HasAuthority)
            Think();

        if (!isFacingTarget || CurrentState == HardCandyState.Observing)
        {
            if ((bool)StandingStill || CurrentState == HardCandyState.Charging)
            {
                Vector3 targetLocation = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);

                if (CurrentState == HardCandyState.Observing)
                    targetLocation = new Vector3(victim.position.x, transform.position.y, victim.position.z);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), rotationLerpTime);
                rotationLerpTime += Time.deltaTime * speedLookup[CurrentState].RotateSpeedMultiplier * 0.01f;

                if (Quaternion.Angle(Quaternion.LookRotation(targetLocation - transform.position, Vector3.up), transform.rotation) <= 1 || DistanceToTargetSquared <= personalBoundaryDistanceSquared)
                {
                    StartedFacingTarget();
                }
            }
        }

        _isGrounded = null; //reset so it will be calculated the next frame if needed
    }

    private void Think()
    {
        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer && (bool)StandingStill != previousStandingStill)
        {
            SetIsStandingStill((bool)StandingStill);
        }

        if ((victim == null || Time.time - setVictimTime > 3) && GameManager.Instance != null)
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

        if (victim != null)
        {
            if (Vector3.SqrMagnitude(victim.transform.position - transform.position) > DiscoveryRadius * DiscoveryRadius)
            {
                SetNewVictim(null);
            }
        }

        if (CurrentState == HardCandyState.Observing && Time.time - thinkAboutVictimTime > 1f)
            ThinkAboutVictim();
        else if (!hasRedirectedCharge && !(bool)StandingStill && CurrentState == HardCandyState.Charging && Time.time - thinkAboutVictimTime > 0.8f)
        {
            ThinkAboutVictim();
            hasRedirectedCharge = true;
        }
        else if (CurrentState == HardCandyState.Fleeing && Time.time - thinkAboutVictimTime > 0.5f)
        {
            ThinkAboutVictim(false);
        }

        if (DistanceToTargetSquared < personalBoundaryDistanceSquared)
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

        if ((standingStillTime > 2.5f || (standingStillTime > 0.4f && Navigation.path.status != NavMeshPathStatus.PathComplete)) && DistanceToTargetSquared > personalBoundaryDistanceSquared)
        {
            standingStillTime = 0;
            ReachedTarget();
        }

        if (CurrentState == HardCandyState.Idle)
        {
            idleTime += Time.deltaTime;

            if (idleTime >= idleGoalTime)
            {
                SetRoaming();
            }
        }

        previousStandingStill = (bool)StandingStill;
    }

    private void ThinkAboutVictim(bool canRedirect = true)
    {
        thinkAboutVictimTime = Time.time;

        NavMeshPath path = new NavMeshPath();
        if (Navigation.CalculatePath(victim.position, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                CurrentState = HardCandyState.Observing;
            }
            else if (canRedirect)
            {
                hasRedirectedCharge = false;
                CurrentState = HardCandyState.Charging;
                TargetPosition = GetChargePosition();

                if (Navigation.isActiveAndEnabled)
                    Navigation.SetDestination(TargetPosition);
            }
        }
        else
        {
            CurrentState = HardCandyState.Observing;
        }
    }

    private void VictimDied(Health sender, CauseOfDeath causeOfDeath)
    {
        SetNewVictim(null);
    }

    private PlayerMovement FindClosestPlayer()
    {
        PlayerMovement closestPlayer = null;
        float closestPlayerDistance = 100000;

        foreach (PlayerMovement player in GameManager.Instance.PlayerCharacters)
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

        return closestPlayer;
    }

    private Health GetHealthForTransform(Transform transform)
    {
        if (!healthLookup.ContainsKey(transform))
            healthLookup.Add(transform, transform.gameObject.GetComponent<Health>());

        return healthLookup[transform];
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

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        Health.OnDied += Died;
    }

    private void UnBindEvents()
    {
        Health.OnDied -= Died;

        if (victim != null && healthLookup.ContainsKey(victim))
            healthLookup[victim].OnDied -= VictimDied;
    }

    private void ReachedTarget()
    {
        if (CurrentState == HardCandyState.Roaming)
        {
            SetIdle();
        }
        else if (CurrentState == HardCandyState.Charging)
        {
            Flee();
        }
        else if (CurrentState == HardCandyState.Fleeing)
        {
            ThinkAboutVictim();
        }
    }

    private void SetIdle()
    {
        idleTime = 0;
        idleGoalTime = Random.Range(0.2f, 2f);
        CurrentState = HardCandyState.Idle;
    }

    private void Flee()
    {
        CurrentState = HardCandyState.Fleeing;
        Vector3? fleePosition = GetRandomPositionWithinDistance(RoamNewTargetRange);
        if (fleePosition == null)
        {
            TargetPosition = transform.position;
        }
        else
        {
            TargetPosition = (Vector3)fleePosition;
        }
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
    private void SetIsStandingStill(bool newValue)
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        standingStillOverride = newValue;
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

    private void SetRoaming()
    {
        idleGoalTime = 0;
        idleTime = 0;
        CurrentState = HardCandyState.Roaming;
        FindNewRoamTarget();
    }

    private void FindNewRoamTarget()
    {
        Vector3? newPosition = GetRandomPositionWithinDistance(RoamNewTargetRange);

        if (newPosition != null)
        {
            SetNewTarget((Vector3)newPosition);
        }
        else
        {
            Debug.LogError("Hard Candy could not find new target position");
        }
    }

    private void SetNewTarget(Vector3 position)
    {
        TargetPosition = position;
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        Destroy(gameObject);
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        Flee();
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        if (UniqueSoundSource.Active)
        {
            FootStepAudio.PlayNext();
        }
    }

    private bool CalculateIsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;
        }

        return false;
    }

    private Vector3 GetChargePosition()
    {
        Vector3 position = victim.position + (victim.position - transform.position).normalized * 2f;
        Ray ray = new Ray(position, Vector3.up * -1);

        if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.NameToLayer("Terrain")))
        {
            if (hit.transform.tag == "Ground")
                position = hit.point;
        }

        return position;
    }

    private Vector3? GetRandomPositionWithinDistance(float distance, int attempt = 0)
    {
        bool foundValue = false;

        Vector3 randomPosition = Random.insideUnitSphere * distance;
        randomPosition.y = transform.position.y;

        Vector3? result = transform.position + randomPosition;

        result += Vector3.up * 0.05f;

        Ray groundRay = new Ray((Vector3)result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            if (groundHit.transform.tag == "Ground")
            {
                foundValue = true;
                result = groundHit.point;
            }
        }

        if (foundValue) //we have found a target position, now we want to check if we can see it
        {
            Ray visionRay = new Ray(transform.position + Vector3.up * 0.3f, (Vector3)result - transform.position + Vector3.up * 0.3f);
            if (Physics.Raycast(visionRay, out RaycastHit visionHit))
            {
                if (Vector3.SqrMagnitude(visionHit.point - (Vector3)result) > 0.5f)
                    foundValue = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == HardCandyState.Charging)
        {
            if (Navigation.velocity.magnitude > speedLookup[CurrentState].RunSpeed * 0.8f)
            {
                Health health = GetHealthForTransform(collision.transform);
                if (health != null)
                {
                    health.TakeDamage(ChargeDamage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(TargetPosition, new Vector3(0.4f, 0.2f, 0.4f));
    }
}

public enum HardCandyState
{
    Roaming, Idle, Searching, Charging, Fleeing, Observing
}

[Serializable]
public class HardCandySpeedSettings
{
    public HardCandyState State;
    public float RotateSpeedMultiplier;
    public float RunSpeed;
}