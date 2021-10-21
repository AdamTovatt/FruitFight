using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class JellyBean : MovingCharacter
{
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

    public delegate void StateChangedHandler(object sender, JellyBeanState newState);
    public event StateChangedHandler OnStateChanged;
    public override event AttackHandler OnAttack;

    private NavMeshAgent navMeshAgent;
    private Rigidbody _rigidbody;
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
            return (bool)_isGrounded;
        }
    }
    private bool? _isGrounded = null;

    public override bool StandingStill { get { return navMeshAgent.velocity.sqrMagnitude == 0 || navMeshAgent.isStopped; } }

    private Health targetHealth
    {
        get
        {
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

    private Vector3 targetPosition;
    private Transform target;
    private Vector3 targetLastSeenPosition;

    private int searchTargetReachedCount = 0;

    private bool knockBack = false;
    private float knockBackTime = 0;

    private bool inRangeForAttack;
    private bool hasAttacked;
    private float lastAttackTime;

    private float personalBoundaryDistanceSquared;

    private void Awake()
    {
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
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        _renderer = JellyBeanModel.GetComponent<Renderer>();

        MaterialSettings = SetMaterialSettings(_renderer);

        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);
        OnStateChanged += (sender, newState) => { JellyBeanStateWasChanged(newState); };
        State = Random.Range(1, 3) > 1 ? JellyBeanState.Idle : JellyBeanState.Roaming;

        OnAttack += (sender, attackPosition, attackSide) => { };

        _rigidbody.isKinematic = true;

        personalBoundaryDistanceSquared = PersonalBoundaryDistance * PersonalBoundaryDistance;
    }

    void Update()
    {
        if (target == null && GameManager.Instance != null)
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
                target = closestPlayer.transform;

                if (targetHealth != null)
                    targetHealth.OnDied += TargetDied;

                State = JellyBeanState.Chasing;
            }
        }

        if (target != null) //has target
        {
            if ((target.position - transform.position).sqrMagnitude < DiscoveryRadius * DiscoveryRadius * 2 || HasVisionOfTransform(target)) //target in range
            {
                targetPosition = target.position; //update target position
            }
            else //target was lost
            {
                targetLastSeenPosition = target.position;

                if (targetHealth != null)
                    targetHealth.OnDied -= TargetDied;

                TargetLost();
            }
        }

        if (navMeshAgent.enabled)
        {
            switch (State)
            {
                case JellyBeanState.Roaming:
                    if (TimeInCurrentState < 5f + randomTimeAddition)
                    {
                        if (navMeshAgent.isOnNavMesh)
                            navMeshAgent.SetDestination(targetPosition);

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
                        targetDistance = new Vector3(targetDistance.x, targetDistance.y - Collider.height, targetDistance.z);
                    else
                        targetDistance = new Vector3(targetDistance.x, targetDistance.y + Collider.height, targetDistance.z);

                    if (targetDistance.sqrMagnitude < personalBoundaryDistanceSquared)
                    {
                        targetPosition = transform.position;

                        if (!inRangeForAttack)
                        {
                            lastAttackTime = Time.time;
                            inRangeForAttack = true;
                            Debug.Log("Now in range");
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

                    if (navMeshAgent.isOnNavMesh)
                        navMeshAgent.SetDestination(targetPosition);
                    break;
                case JellyBeanState.Confused:
                    if (navMeshAgent.isOnNavMesh)
                        navMeshAgent.SetDestination(targetPosition);

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
            if ((_rigidbody.velocity.sqrMagnitude < 0.1f && timeSinceKnockBack > 0.2f) || timeSinceKnockBack > 2f)
            {
                if (Health.CurrentHealth <= 0)
                {
                    Die();
                }

                knockBack = false;
                _rigidbody.isKinematic = true;
                navMeshAgent.enabled = true;
            }
        }

        if (!knockBack)
            _isGrounded = null; //reset isGrounded so it is calculated next time someone needs it
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        if (causeOfDeath == CauseOfDeath.Water) //if cause of death is not water we will handle it ourselves in the update method, this is so the jelly bean will die only after finishing sliding
            Die();
    }

    private void TargetLost()
    {
        if (this != null)
        {
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
        OnAttack?.Invoke(this, punchPosition, side);

        Instantiate(PunchSoundEffectPrefab, punchPosition, Quaternion.identity);

        targetHealth.TakeDamage(PunchDamage);

        hasAttacked = true;
        lastAttackTime = Time.time;
    }

    public override void WasAttacked(Vector3 attackOrigin, Transform attackingTransform, float attackStrength)
    {
        soundSource.Play("hurt");

        if (!knockBack)
        {
            _rigidbody.isKinematic = false;
            navMeshAgent.enabled = false;
            _rigidbody.AddExplosionForce(attackStrength * 50f, attackOrigin, attackStrength);
            _rigidbody.AddForce(transform.up, ForceMode.VelocityChange);
            knockBack = true;
            knockBackTime = Time.time;
        }
    }

    public void Die()
    {
        Debug.Log("jelly bean died");
        GameObject deadJellyBean = Instantiate(DeadPrefab, transform.position + new Vector3(0, 0.7f, 0), transform.rotation);
        SetMaterialSettings(deadJellyBean.GetComponentInChildren<Renderer>(), MaterialSettings);
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
        lastStateChange = Time.time;
        randomTimeAddition = Random.Range(randomTimeMin, randomTimeMax);

        StatusText.text = newState.ToString();

        if (navMeshAgent.isOnNavMesh)
        {
            switch (newState)
            {
                case JellyBeanState.Roaming:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = RoamSpeed;
                    targetPosition = GetNewTargetPosition(RoamNewTargetRange);
                    break;
                case JellyBeanState.Idle:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(transform.position);
                    break;
                case JellyBeanState.Chasing:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = ChaseSpeed;
                    break;
                case JellyBeanState.Confused:
                    navMeshAgent.isStopped = false;
                    break;
                case JellyBeanState.Searching:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = SearchSpeed;
                    targetPosition = targetLastSeenPosition;
                    navMeshAgent.SetDestination(targetPosition);
                    break;
                default:
                    throw new System.Exception("No such state for: " + ToString());
            }
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
