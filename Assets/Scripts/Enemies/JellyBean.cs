using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class JellyBean : MovingCharacter
{
    public static List<Texture2D> JellyBeanCoatings;

    public TextMeshPro StatusText;
    public GameObject DeadPrefab;

    public float RoamSpeed = 1f;
    public float RoamNewTargetRange = 5f;
    public float ChaseSpeed = 3f;
    public float SearchSpeed = 2f;
    public float PersonalBoundaryDistance = 0.1f;
    public float DiscoveryRadius = 4f;
    public float DistanceToGround = 0.1f;
    public float AttackInitialWaitTime = 0.4f;
    public float AttackCooldown = 1f;
    public float PunchDamage = 10f;

    public Transform JellyBeanModel;
    public List<Texture2D> CoatingTextures;
    public Health Health;
    public CapsuleCollider Collider;
    public Rigidbody Rigidbody;
    public NavMeshAgent NavMeshAgent;

    public JellyBeanMaterialSettings MaterialSettings { get; private set; }

    public JellyBeanState State
    {
        get { return _state; }
        private set { OnStateChanged?.Invoke(this, value); _state = value; }
    }

    private JellyBeanState _state;

    public float TimeInCurrentState
    {
        get
        {
            return Time.time - lastStateChange;
        }
    }

    public delegate void BecameGroundedHandler();
    public event BecameGroundedHandler OnBecameGrounded;

    public delegate void StateChangedHandler(object sender, JellyBeanState newState);
    public event StateChangedHandler OnStateChanged;
    public override event AttackHandler OnAttack;

    private Renderer _renderer;
    private SoundSource soundSource;

    private float lastStateChange;
    private float randomTimeAddition;
    private bool remembersPlayer = false;

    private float randomTimeMin = 0;
    private float randomTimeMax = 0;

    public override bool StopFootSetDefault { get { return knockBack; } }

    public override bool IsGrounded
    {
        get
        {
            if (_isGrounded == null)
                _isGrounded = CalculateIsGrounded();
            bool grounded = (bool)_isGrounded;

            if (grounded && !lastGrounded)
            {
                OnBecameGrounded?.Invoke();
                OnBecameGrounded = null;
            }

            return grounded;
        }
    }
    private bool? _isGrounded = null;
    private bool lastGrounded = false;

    public override bool? StandingStill { get { return NavMeshAgent.velocity.sqrMagnitude == 0 || NavMeshAgent.isStopped; } }

    private Health targetHealth
    {
        get
        {
            if (target == null)
                return null;

            if (target == _targetHealthTarget && hasGottenTargetHealth)
                return _targetHealth;

            _targetHealthTarget = target;
            _targetHealth = _targetHealthTarget.gameObject.GetComponent<Health>();
            hasGottenTargetHealth = true;

            return _targetHealth;
        }
    }

    private Health _targetHealth; //the health component of the target
    private Transform _targetHealthTarget; //the last target that the health component was fetched for
    private bool hasGottenTargetHealth; //if the target health has been fetched

    private UniqueSoundSource uniqueSoundSource;
    private FootStepAudioSource footStepAudioSource;

    [SyncVar]
    private Vector3 targetPosition;
    private Transform target;
    [SyncVar]
    private Vector3 targetLastSeenPosition;
    [SyncVar]
    private float currentSpeed;

    private int searchTargetReachedCount = 0;

    private bool knockBack = false;
    private float knockBackTime = 0;

    private bool inRangeForAttack;
    private bool hasAttacked;
    private float lastAttackTime;

    private float personalBoundaryDistanceSquared;

    private Dictionary<Transform, Health> transformHealths = new Dictionary<Transform, Health>();

    private Dictionary<uint, Transform> netIdToPlayerDictionary = new Dictionary<uint, Transform>();
    private Dictionary<Transform, uint> playerToNetIdDictionary = new Dictionary<Transform, uint>();

    private float setTargetTime;

    private float syncPositionTime;

    private void Awake()
    {
        if (JellyBeanCoatings == null)
        {
            JellyBeanCoatings = CoatingTextures;
        }

        uniqueSoundSource = gameObject.GetComponent<UniqueSoundSource>();
        footStepAudioSource = gameObject.GetComponent<FootStepAudioSource>();
        soundSource = gameObject.GetComponent<SoundSource>();
        Health.OnDied += Died;
    }

    private void OnDestroy()
    {
        Health.OnDied -= Died;
    }

    void Start()
    {
        _renderer = JellyBeanModel.GetComponent<Renderer>();

        MaterialSettings = SetMaterialSettings(_renderer);

        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);
        OnStateChanged += (sender, newState) => { JellyBeanStateWasChanged(newState); };
        State = Random.Range(1, 3) > 1 ? JellyBeanState.Idle : JellyBeanState.Roaming;

        OnAttack += (attackPosition, attackSide) => { };

        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;

        if (CustomNetworkManager.IsOnlineSession && !CustomNetworkManager.Instance.IsServer)
            CmdInvokeSyncTexture();
    }

    void Update()
    {
        if (CustomNetworkManager.HasAuthority) //is offline or is server
        {
            if ((target == null || Time.time - setTargetTime > 3) && GameManager.Instance != null)
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

                if (closestPlayer != null)
                {
                    Health health = GetHealthForTransform(closestPlayer.transform);

                    if (health != null && !health.IsDead)
                    {
                        SetTarget(health, closestPlayer.transform, JellyBeanState.Chasing);
                    }
                }
            }

            syncPositionTime += Time.deltaTime;

            if (syncPositionTime > 5)
            {
                //ForceSyncPosition();
                syncPositionTime = 0;
            }
        }

        NavMeshAgent.speed = currentSpeed; //for some reason it gets the wrong value sometime so we force it to be the right value

        if (target != null) //has target
        {
            if ((target.position - transform.position).sqrMagnitude < DiscoveryRadius * DiscoveryRadius * 2 || HasVisionOfTransform(target)) //target in range
            {
                targetPosition = target.position; //update target position
            }
            else //target was lost
            {
                if (CustomNetworkManager.HasAuthority) //only do this if we are offline or if we are the server
                {
                    if (targetHealth != null)
                        targetHealth.OnDied -= TargetDied;

                    if (CustomNetworkManager.IsOnlineSession)
                        RpcTargetLost(); //send to client

                    TargetLost(); //perform ourselves
                }
            }
        }

        if (NavMeshAgent.enabled)
        {
            switch (State)
            {
                case JellyBeanState.Roaming:
                    if (TimeInCurrentState < 5f + randomTimeAddition)
                    {
                        if (NavMeshAgent.isOnNavMesh)
                            NavMeshAgent.SetDestination(targetPosition);

                        if (Vector3.Distance(targetPosition, transform.position) < PersonalBoundaryDistance)
                        {
                            State = JellyBeanState.Roaming;
                        }
                    }
                    else
                    {
                        if (Random.Range(0, 10) < 2)
                            State = JellyBeanState.Confused;
                        else
                            State = JellyBeanState.Idle;
                    }
                    break;
                case JellyBeanState.Idle:
                    if (TimeInCurrentState > 2f + randomTimeAddition)
                    {
                        if (remembersPlayer)
                        {
                            State = JellyBeanState.Searching;
                        }
                        else
                        {
                            State = JellyBeanState.Roaming;
                        }
                    }
                    else
                    {
                        //do nothing
                    }
                    break;
                case JellyBeanState.Chasing:
                    Vector3 targetDistance = targetPosition - transform.position;

                    if (targetDistance.y > 0)
                    {
                        targetDistance = new Vector3(targetDistance.x, Mathf.Min(Mathf.Abs(targetDistance.y - Collider.height), Mathf.Abs(targetDistance.y)), targetDistance.z);
                    }

                    if (targetDistance.sqrMagnitude < personalBoundaryDistanceSquared)
                    {
                        targetPosition = transform.position;

                        if (!inRangeForAttack)
                        {
                            lastAttackTime = Time.time;
                            inRangeForAttack = true;
                        }
                        else
                        {
                            float timeRequirement = hasAttacked ? AttackCooldown : AttackInitialWaitTime;
                            if (Time.time - lastAttackTime > timeRequirement)
                            {
                                Attack();
                            }
                        }
                    }
                    else
                    {
                        hasAttacked = false;
                        inRangeForAttack = false;
                    }

                    if (NavMeshAgent.isOnNavMesh)
                        NavMeshAgent.SetDestination(targetPosition);
                    break;
                case JellyBeanState.Confused:
                    if (NavMeshAgent.isOnNavMesh)
                        NavMeshAgent.SetDestination(targetPosition);

                    if (TimeInCurrentState > 1f + randomTimeAddition)
                    {
                        State = JellyBeanState.Searching;
                    }
                    break;
                case JellyBeanState.Searching:
                    if ((targetPosition - transform.position).sqrMagnitude < personalBoundaryDistanceSquared)
                    {
                        if (searchTargetReachedCount < 5 + Random.Range(0, 1)) //if we haven't reached our search target to many times we want to search again
                        {
                            targetLastSeenPosition = GetNewTargetPosition(RoamNewTargetRange / 2);
                            searchTargetReachedCount++; //so the jelly bean will stop searching eventually
                            State = JellyBeanState.Confused;
                        }
                        else //stop searching, we've looked for long enough
                        {
                            State = JellyBeanState.Idle;
                        }
                    }

                    if (TimeInCurrentState > 12f + randomTimeAddition)
                    {
                        State = JellyBeanState.Idle;
                    }
                    break;
                default:
                    break;
            }
        }

        if (knockBack)
        {
            float timeSinceKnockBack = Time.time - knockBackTime;
            if ((Rigidbody.velocity.sqrMagnitude < 0.1f && timeSinceKnockBack > 0.2f) || timeSinceKnockBack > 2f)
            {
                if (Health.CurrentHealth <= 0)
                {
                    Die();
                }

                knockBack = false;
                Rigidbody.isKinematic = true;
                NavMeshAgent.enabled = true;
            }
        }

        if (!knockBack)
        {
            lastGrounded = _isGrounded ?? false;
            _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it
        }
    }

    public void SetSummoningCompleted()
    {
        if(CustomNetworkManager.IsOnlineSession)
        {
            if(CustomNetworkManager.Instance.IsServer)
            {
                RpcSetSummoningCompleted();
                PerformSetSummoningCompleted();
            }
        }
        else
        {
            PerformSetSummoningCompleted();
        }
    }

    [ClientRpc]
    private void RpcSetSummoningCompleted()
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformSetSummoningCompleted();
    }

    private void PerformSetSummoningCompleted()
    {
        Rigidbody.isKinematic = true;
        NavMeshAgent.enabled = true;
    }

    public void SetWasSummoned()
    {
        if(CustomNetworkManager.IsOnlineSession)
        {
            if(CustomNetworkManager.Instance.IsServer)
            {
                RpcSetWasSummoned();
                PerformSetWasSummoned();
            }
        }
        else
        {
            PerformSetWasSummoned();
        }
    }

    [ClientRpc]
    private void RpcSetWasSummoned()
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformSetWasSummoned();
    }

    private void PerformSetWasSummoned()
    {
        Rigidbody.isKinematic = false;
        NavMeshAgent.enabled = false;
    }

    private void ForceSyncPosition()
    {
        RpcSetPosition(transform.position, targetPosition);
    }

    [ClientRpc]
    private void RpcSetPosition(Vector3 position, Vector3 targetPosition)
    {
        NavMeshAgent.Warp(position);
        this.targetPosition = targetPosition;
    }

    public void SetTarget(Health healthOfTarget, Transform newTarget, JellyBeanState newState)
    {
        healthOfTarget.OnDied += TargetDied;
        setTargetTime = Time.time;

        if (!playerToNetIdDictionary.ContainsKey(newTarget))
            playerToNetIdDictionary.Add(newTarget, newTarget.GetComponent<NetworkIdentity>().netId);

        if (CustomNetworkManager.IsOnlineSession)
            RpcSetTarget(playerToNetIdDictionary[newTarget], (int)newState);

        PerformSetTarget(newTarget, newState);
    }

    private void PerformSetTarget(Transform newTarget, JellyBeanState newState)
    {
        target = newTarget;
        State = newState;
    }

    [ClientRpc]
    private void RpcSetTarget(uint netId, int newState)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        if (!netIdToPlayerDictionary.ContainsKey(netId))
            netIdToPlayerDictionary.Add(netId, NetworkClient.spawned[netId].transform);

        PerformSetTarget(netIdToPlayerDictionary[netId], (JellyBeanState)newState);
    }

    private Health GetHealthForTransform(Transform transform)
    {
        if (!transformHealths.ContainsKey(transform))
            transformHealths.Add(transform, transform.gameObject.GetComponent<Health>());

        return transformHealths[transform];
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        if (causeOfDeath == CauseOfDeath.Water) //if cause of death is not water we will handle it ourselves in the update method, this is so the jelly bean will die only after finishing sliding
            Die();
    }

    [ClientRpc]
    private void RpcTargetLost()
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        TargetLost();
    }

    private void TargetLost()
    {
        if (this != null)
        {
            if (target != null)
                targetLastSeenPosition = target.position;
            else
                targetLastSeenPosition = transform.position;

            target = null;
            targetPosition = transform.position;
            searchTargetReachedCount = 0; //needed later to stop searching for player at last seen position
            State = JellyBeanState.Confused;
        }
    }

    private void TargetDied(Health health, CauseOfDeath causeOfDeath)
    {
        TargetLost();
    }

    private void Attack()
    {
        AttackSide side = Random.Range(0, 2) > 0 ? AttackSide.Right : AttackSide.Left;
        Vector3 punchPosition = transform.position + transform.forward * 0.2f + transform.up * 0.5f;
        punchPosition += transform.right * 0.1f * (side == AttackSide.Right ? 1f : -1f);
        OnAttack?.Invoke(punchPosition, side);

        Instantiate(PunchSoundEffectPrefab, punchPosition, Quaternion.identity);

        targetHealth.TakeDamage(PunchDamage);

        hasAttacked = true;
        lastAttackTime = Time.time;
    }

    [ClientRpc]
    private void RpcWasAttacked(Vector3 attackOrigin, float attackStrength)
    {
        if (!CustomNetworkManager.Instance.IsServer)
            PerformWasAttacked(attackOrigin, attackStrength); //we should only do this as a client since the server has already done it locally
    }

    [Command(requiresAuthority = false)]
    private void CmdWasAttacked(Vector3 attackOrigin, float attackStrength)
    {
        PerformWasAttacked(attackOrigin, attackStrength); //only do this as a server since the client has aldready done it locally
    }

    public void SetKnockback(Vector3 forceOrigin, float forceStrength)
    {
        Rigidbody.isKinematic = false;
        NavMeshAgent.enabled = false;
        Rigidbody.AddExplosionForce(forceStrength * 50f, forceOrigin, forceStrength);
        Rigidbody.AddForce(transform.up, ForceMode.VelocityChange);
        knockBack = true;
        knockBackTime = Time.time;
    }

    private void PerformWasAttacked(Vector3 attackOrigin, float attackStrength)
    {
        if (!knockBack)
        {
            soundSource.Play("hurt");

            SetKnockback(attackOrigin, attackStrength);
        }
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcWasAttacked(attackOrigin, attackStrength); //tell the other player to perform attack
            }
            else
            {
                CmdWasAttacked(attackOrigin, attackStrength); //tell the other player to perform attack
            }
        }

        PerformWasAttacked(attackOrigin, attackStrength); //we should do this locally regardless of if it's an online session or not
    }

    public void Die()
    {
        if (CustomNetworkManager.HasAuthority)
        {
            GameObject deadJellyBean = Instantiate(DeadPrefab, transform.position + new Vector3(0, 0.7f, 0), transform.rotation);
            DeadJellyBean deadBean = deadJellyBean.GetComponent<DeadJellyBean>();
            deadBean.AssignTexture(SetMaterialSettings(deadBean.BeanRenderer, MaterialSettings).MainTexture);

            if (CustomNetworkManager.IsOnlineSession)
                NetworkServer.Spawn(deadJellyBean);
        }

        Destroy(gameObject);
    }

    private Vector3 GetNewTargetPosition(float targetDistance)
    {
        Vector3 randomPosition = Random.insideUnitSphere * targetDistance;
        randomPosition.y = transform.position.y;

        Vector3 result = transform.position + randomPosition;

        result += Vector3.up * 0.05f;

        Ray groundRay = new Ray(result, Vector3.up * -1);
        if (Physics.Raycast(groundRay, out RaycastHit groundHit, LayerMask.NameToLayer("Terrain")))
        {
            result = groundHit.point;
        }
        else
        {
            result.y = transform.position.y;
        }

        return result;
    }

    [Command(requiresAuthority = false)]
    private void CmdInvokeSyncTexture()
    {
        RpcSetTextureFromServer(MaterialSettings.MainTexture);
    }

    [ClientRpc]
    private void RpcSetTextureFromServer(int texture)
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        SetMaterialSettings(_renderer, new JellyBeanMaterialSettings(texture));
    }

    private JellyBeanMaterialSettings SetMaterialSettings(Renderer renderer, JellyBeanMaterialSettings materialSettings = null)
    {
        if (materialSettings == null)
        {
            materialSettings = new JellyBeanMaterialSettings(Random.Range(0, CoatingTextures.Count));
        }

        renderer.material.mainTexture = CoatingTextures[materialSettings.MainTexture];

        return materialSettings;
    }

    private bool HasVisionOfTransform(Transform target)
    {
        Ray ray = new Ray(transform.position + transform.up * 0.5f, target.position - transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == target;
        }

        return false;
    }

    public void JellyBeanStateWasChanged(JellyBeanState newState)
    {
        if (!CustomNetworkManager.HasAuthority)
            return;

        lastStateChange = Time.time;
        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);

        StatusText.text = newState.ToString();

        if (NavMeshAgent.isOnNavMesh)
        {
            switch (newState)
            {
                case JellyBeanState.Roaming:
                    UpdateStateProperties(newState, GetNewTargetPosition(RoamNewTargetRange));
                    break;
                case JellyBeanState.Idle:
                    UpdateStateProperties(newState, Vector3.zero);
                    break;
                case JellyBeanState.Chasing:
                    UpdateStateProperties(newState, Vector3.zero);
                    break;
                case JellyBeanState.Confused:
                    UpdateStateProperties(newState, Vector3.zero);
                    break;
                case JellyBeanState.Searching:
                    UpdateStateProperties(newState, targetLastSeenPosition);
                    break;
                default:
                    throw new System.Exception("No such state for: " + ToString());
            }
        }
    }

    private void UpdateStateProperties(JellyBeanState newState, Vector3 position)
    {
        if (!CustomNetworkManager.HasAuthority)
            return;

        PerformUpdateStateProperties(newState, position);

        if (CustomNetworkManager.IsOnlineSession)
            RpcUpdateStateProperties((int)newState, position);
    }

    private void RpcUpdateStateProperties(int newState, Vector3 position)
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        State = (JellyBeanState)newState;
        PerformUpdateStateProperties((JellyBeanState)newState, position);
    }

    private void PerformUpdateStateProperties(JellyBeanState newState, Vector3 position)
    {
        switch (newState)
        {
            case JellyBeanState.Roaming:
                NavMeshAgent.isStopped = false;
                NavMeshAgent.speed = RoamSpeed;
                currentSpeed = RoamSpeed;
                targetPosition = position;
                break;
            case JellyBeanState.Idle:
                NavMeshAgent.isStopped = false;
                NavMeshAgent.SetDestination(transform.position);
                break;
            case JellyBeanState.Chasing:
                NavMeshAgent.isStopped = false;
                NavMeshAgent.speed = ChaseSpeed;
                currentSpeed = ChaseSpeed;
                break;
            case JellyBeanState.Confused:
                NavMeshAgent.isStopped = false;
                break;
            case JellyBeanState.Searching:
                NavMeshAgent.isStopped = false;
                NavMeshAgent.speed = SearchSpeed;
                targetPosition = position;
                NavMeshAgent.SetDestination(targetPosition);
                break;
            default:
                throw new System.Exception("No such state for: " + ToString());
        }
    }

    private bool CalculateIsGrounded()
    {
        if (knockBack)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform.tag == "Ground" && hit.distance <= DistanceToGround;
        }

        return false;
    }

    public override void StepWasTaken(Vector3 stepPosition)
    {
        if (uniqueSoundSource.Active)
        {
            footStepAudioSource.PlayNext();
        }
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
        {
            try
            {
                FindObjectOfType<MainMenuManager>().ActivateObjectWithDelay(gameObject, 0.2f);
            }
            catch { } //we wont do anything here
        }
    }
}

public class JellyBeanMaterialSettings
{
    public int MainTexture { get; set; }

    public JellyBeanMaterialSettings(int mainTexture)
    {
        MainTexture = mainTexture;
    }
}

public enum JellyBeanState
{
    Roaming, Idle, Chasing, Confused, Searching
}
