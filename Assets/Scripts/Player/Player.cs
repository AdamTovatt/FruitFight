using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public int JellyBeans;

    [SyncVar]
    public int Coins;

    public float PlayerRespawnTime = 5;
    public int DeathCost = 5;
    public float RunParticlesEmissionRate = 30f;

    public ParticleSystem RunParticles;

    public delegate void JellyBeansUpdatedHandler(int newAmount);
    public event JellyBeansUpdatedHandler OnJellyBeansUpdated;

    public delegate void CoinsUpdatedHandler(int newAmount);
    public event CoinsUpdatedHandler OnCoinsUpdated;

    public delegate void OnPickedUpItemHandler(Holdable holdableItem);
    public event OnPickedUpItemHandler OnPickedUpItem;

    public delegate void OnDroppedItemHandler();
    public event OnDroppedItemHandler OnDroppedItem;

    public delegate void OnStartedCastingHandler(Vector3 castingPosition, float castingRadius);
    public event OnStartedCastingHandler OnStartedCasting;

    public delegate void OnStoppedCastingHandler();
    public event OnStoppedCastingHandler OnStoppedCasting;

    public GameObject BeamOfLightPrefab;
    public GameObject AngelPrefab;

    public AverageVelocityKeeper VelocityKeeper;
    public TemporaryMeshReplacer MeshReplacer;
    public Health Health;
    public PlayerMovement Movement;
    public PlayerNetworkCharacter NetworkCharacter;

    private int magicProjectileId;
    private ProjectileConfigurationEntry magicProjectileConfigurationEntry;
    private List<MagicCharge> currentMagicCharges = new List<MagicCharge>();

    private List<float> previousDeathTimes = new List<float>();

    private void Start()
    {
        InvokeJellyBeansUpdated(JellyBeans);
        InvokeCoinsUpdated(Coins);

        Health.OnDied += OnDied;

        if (CustomNetworkManager.IsOnlineSession)
        {
            this.CallWithDelay(ForceSyncPosition, 2);
        }

        if (magicProjectileConfigurationEntry != null)
            magicProjectileConfigurationEntry.Load();
    }

    private void Update()
    {
        ParticleSystem.EmissionModule emission = RunParticles.emission;
        emission.rateOverTime = RunParticlesEmissionRate * VelocityKeeper.Velocity;
    }

    private void OnDestroy()
    {
        Health.OnDied -= OnDied;
    }

    private void OnDied(Health sender, CauseOfDeath causeOfDeath)
    {
        GroundedPositionInformation groundedPosition = new GroundedPositionInformation(transform, 0, transform.position);

        if (Movement.isActiveAndEnabled && Movement.ChargingProjectile)
            Movement.StopChargeProjectile(false);

        if (!CustomNetworkManager.IsOnlineSession || NetworkCharacter.IsLocalPlayer)
        {
            previousDeathTimes.Add(Time.time);
            if (previousDeathTimes.Count > 5)
                previousDeathTimes.RemoveAt(0);

            //if (previousDeathTimes.Count >= 5 && (previousDeathTimes.Max() - previousDeathTimes.Min()) - PlayerRespawnTime * 5 < 10) //if we have died 5 times within 10 seconds
            //{

            if (Movement.PreviousGroundedPositions.Keys.Count > 1)
            {
                Movement.PreviousGroundedPositions.Remove(Movement.PreviousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key);
                previousDeathTimes.Clear();
            }

            if (Movement.PreviousGroundedPositions.Count > 0)
            {
                groundedPosition = Movement.PreviousGroundedPositions[Movement.PreviousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key];
            }

            //}
            //else
            //{
            //    if (causeOfDeath == CauseOfDeath.Water)
            //    {
            //        newPosition = Movement.LastGroundedPosition.Position;
            //    }
            //}

            Movement.ControlsEnabled = false;

            if (!CustomNetworkManager.IsOnlineSession)
            {
                Instantiate(AngelPrefab, transform.position, transform.rotation);
            }
            else
            {
                NetworkCharacter.SpawnAngel();
            }

            MeshReplacer.ReplaceMesh(causeOfDeath != CauseOfDeath.Water);

            RemoveItem(AbsorbableItemType.JellyBean, JellyBeans >= DeathCost ? DeathCost : JellyBeans);
            RemoveItem(AbsorbableItemType.Coin, Coins >= DeathCost * 2 ? DeathCost * 2 : Coins);

            this.CallWithDelay(() => { Respawn(groundedPosition); }, PlayerRespawnTime);
        }
    }

    public void ForceSyncPosition()
    {
        Debug.Log("Sync position");
        if (NetworkCharacter.IsLocalPlayer)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcForceSyncPosition(transform.position);
                transform.position = transform.position += new Vector3(0, 0.1f, 0);
            }
            else
            {
                CmdForceSyncPosition(transform.position);
                transform.position = transform.position += new Vector3(0, 0.1f, 0);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdForceSyncPosition(Vector3 newPosition)
    {
        Debug.Log("command from client on server");
        transform.position = newPosition;
    }

    [ClientRpc]
    private void RpcForceSyncPosition(Vector3 newPosition)
    {
        Debug.Log("rpc from server: " + newPosition.ToString());
        Debug.Log("Current pos: " + transform.position.ToString());
        transform.position = newPosition;
    }

    public void PickedUpItem(Holdable item)
    {
        OnPickedUpItem?.Invoke(item);
    }

    public void DroppedItem()
    {
        OnDroppedItem?.Invoke();
    }

    public void Respawn(GroundedPositionInformation groundedPosition)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcRespawn(groundedPosition.Transform.position + groundedPosition.RelativePosition);
            }
            else
            {
                CmdRespawn(groundedPosition.Transform.position + groundedPosition.RelativePosition);
            }
        }
        else
        {
            PerformRespawn(groundedPosition.Transform.position + groundedPosition.RelativePosition);
        }
    }

    [Command]
    private void CmdRespawn(Vector3 newPosition)
    {
        RpcRespawn(newPosition);
    }

    [ClientRpc]
    private void RpcRespawn(Vector3 newPosition)
    {
        PerformRespawn(newPosition);
    }

    private void PerformRespawn(Vector3 newPosition)
    {
        transform.position = newPosition;
        BeamOfLight beamOfLight = Instantiate(BeamOfLightPrefab, newPosition, transform.rotation).GetComponent<BeamOfLight>();

        beamOfLight.OnReachedPeak += () =>
        {
            MeshReplacer.GoBackToNormal();

            if (!CustomNetworkManager.IsOnlineSession || NetworkCharacter.IsLocalPlayer)
                Movement.ControlsEnabled = true;

            Health.Resurrect();
        };
    }

    public void AbsorbedItem(AbsorbableItemType itemType)
    {
        switch (itemType)
        {
            case AbsorbableItemType.JellyBean:
                JellyBeans++;
                InvokeJellyBeansUpdated(JellyBeans);
                break;
            case AbsorbableItemType.Coin:
                Coins++;
                InvokeCoinsUpdated(Coins);
                break;
            default:
                break;
        }
    }

    public void RemoveItem(AbsorbableItemType itemType, int amount)
    {
        if (CustomNetworkManager.HasAuthority)
        {
            PerformRemoveItem((int)itemType, amount);
        }
        else
        {
            CmdRemoveItem((int)itemType, amount);
        }
    }

    [Command]
    private void CmdRemoveItem(int itemType, int amount)
    {
        PerformRemoveItem(itemType, amount);
    }

    private void PerformRemoveItem(int itemType, int amount)
    {
        AbsorbableItemType itemTypeEnum = (AbsorbableItemType)itemType;

        switch (itemTypeEnum)
        {
            case AbsorbableItemType.JellyBean:
                JellyBeans -= amount;
                InvokeJellyBeansUpdated(JellyBeans);
                break;
            case AbsorbableItemType.Coin:
                Coins -= amount;
                InvokeCoinsUpdated(Coins);
                break;
            default:
                break;
        }
    }

    public void InvokeJellyBeansUpdated(int jellyBeans)
    {
        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer)
            RpcInvokeJellyBeansUpdated(jellyBeans);
        else
            OnJellyBeansUpdated?.Invoke(jellyBeans);
    }

    public void InvokeCoinsUpdated(int coins)
    {
        if (CustomNetworkManager.IsOnlineSession && CustomNetworkManager.Instance.IsServer)
            RpcInvokeCoinsUpdated(coins);
        else
            OnCoinsUpdated?.Invoke(coins);
    }

    [ClientRpc]
    private void RpcInvokeJellyBeansUpdated(int jellyBeans)
    {
        OnJellyBeansUpdated?.Invoke(jellyBeans);
    }

    [ClientRpc]
    private void RpcInvokeCoinsUpdated(int coins)
    {
        OnCoinsUpdated?.Invoke(coins);
    }

    public void TurnOnRunParticles()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                PerformTurnOnRunParticles();
                RpcTurnOnRunParticles();
            }
            else
            {
                CmdTurnOnRunParticles();
            }
        }
        else
        {
            PerformTurnOnRunParticles();
        }
    }

    [ClientRpc]
    private void RpcTurnOnRunParticles()
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        PerformTurnOnRunParticles();
    }

    [Command(requiresAuthority = false)]
    private void CmdTurnOnRunParticles()
    {
        PerformTurnOnRunParticles();
    }

    private void PerformTurnOnRunParticles()
    {
        RunParticles.Play(true);
    }

    public void TurnOffRunParticles()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                PerformTurnOffRunParticles();
                RpcTurnOffRunParticles();
            }
            else
            {
                CmdTurnOffRunParticles();
            }
        }
        else
        {
            PerformTurnOffRunParticles();
        }
    }

    [ClientRpc]
    private void RpcTurnOffRunParticles()
    {
        if (CustomNetworkManager.HasAuthority)
            return;

        PerformTurnOffRunParticles();
    }

    [Command(requiresAuthority = false)]
    private void CmdTurnOffRunParticles()
    {
        PerformTurnOffRunParticles();
    }

    private void PerformTurnOffRunParticles()
    {
        RunParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void StartCasting(float projectileChargeTime, float punchHeight, float castingSize)
    {
        PerformStartCasting(projectileChargeTime, punchHeight, castingSize);

        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
                RpcStartCasting(projectileChargeTime, punchHeight, castingSize);
            else
                CmdStartCasting(projectileChargeTime, punchHeight, castingSize);
        }
    }

    [ClientRpc]
    private void RpcStartCasting(float projectileChargeTime, float punchHeight, float castingSize)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformStartCasting(projectileChargeTime, punchHeight, castingSize);
    }

    [Command(requiresAuthority = false)]
    private void CmdStartCasting(float projectileChargeTime, float punchHeight, float castingSize)
    {
        PerformStartCasting(projectileChargeTime, punchHeight, castingSize);
    }

    private void PerformStartCasting(float projectileChargeTime, float punchHeight, float castingSize)
    {
        Vector3 shootOriginHeightOffset = transform.up * punchHeight;
        MagicCharge magicCharge = Instantiate(magicProjectileConfigurationEntry.Charge, transform.position + transform.forward * 0.5f + shootOriginHeightOffset, Quaternion.Euler(90, 0, 0), transform).GetComponent<MagicCharge>();
        magicCharge.Initialize(projectileChargeTime);
        currentMagicCharges.Add(magicCharge);

        OnStartedCasting?.Invoke(shootOriginHeightOffset, castingSize);
    }

    public void StopCasting(bool disappearInstantly)
    {
        PerformStopCasting(disappearInstantly);

        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
                RpcStopCasting(disappearInstantly);
            else
                CmdStopCasting(disappearInstantly);
        }
    }

    [ClientRpc]
    private void RpcStopCasting(bool disappearInstantly)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformStopCasting(disappearInstantly);
    }

    [Command(requiresAuthority = false)]
    private void CmdStopCasting(bool disappearInstantly)
    {
        PerformStopCasting(disappearInstantly);
    }

    private void PerformStopCasting(bool disappearInstantly)
    {
        foreach (MagicCharge charge in currentMagicCharges)
        {
            charge.Cancel(disappearInstantly);
        }
        currentMagicCharges.Clear();

        OnStoppedCasting?.Invoke();
    }

    public void ShootProjectile(float punchHeight, float projectileLifeTime)
    {
        if (Movement.MagicSettings.ConsumeJellyBeans)
            RemoveItem(AbsorbableItemType.JellyBean, 1);

        Vector3 shootOrigin = transform.position + transform.forward * 0.5f + transform.up * punchHeight;

        Vector3 shooterForward = transform.forward; //we take the numerical values here and send them to the other player so the projectiles will take the same trajectory
        Vector3 shooterPosition = transform.position;

        PerformShootProjectile(shootOrigin, shooterForward, shooterPosition, projectileLifeTime);

        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
                RpcShootProjectile(shootOrigin, shooterForward, shooterPosition, projectileLifeTime);
            else
                CmdShootProjectile(shootOrigin, shooterForward, shooterPosition, projectileLifeTime);
        }
    }

    [ClientRpc]
    private void RpcShootProjectile(Vector3 shootOrigin, Vector3 shooterForward, Vector3 shooterPosition, float projectileLifeTime)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformShootProjectile(shootOrigin, shooterForward, shooterPosition, projectileLifeTime);
    }

    [Command(requiresAuthority = false)]
    private void CmdShootProjectile(Vector3 shootOrigin, Vector3 shooterForward, Vector3 shooterPosition, float projectileLifeTime)
    {
        PerformShootProjectile(shootOrigin, shooterForward, shooterPosition, projectileLifeTime);
    }

    private void PerformShootProjectile(Vector3 shootOrigin, Vector3 shooterForward, Vector3 shooterPosition, float projectileLifeTime)
    {
        MagicProjectile projectile = Instantiate(magicProjectileConfigurationEntry.Projectile, shootOrigin, transform.rotation).GetComponent<MagicProjectile>();
        projectile.Shoot(shootOrigin, transform, shooterForward, shooterPosition, projectileLifeTime);
    }

    public void SetMagicProjectileId(int projectileId)
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
                RpcSetMagicProjectileId(projectileId);
            else
                CmdSetMagicProjectileId(projectileId);
        }

        PerformSetMagicProjectileId(projectileId);
    }

    [ClientRpc]
    private void RpcSetMagicProjectileId(int projectileId)
    {
        if (CustomNetworkManager.Instance.IsServer)
            return;

        PerformSetMagicProjectileId(projectileId);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetMagicProjectileId(int projectileId)
    {
        PerformSetMagicProjectileId(projectileId);
    }

    private void PerformSetMagicProjectileId(int projectileId)
    {
        magicProjectileId = projectileId;
        magicProjectileConfigurationEntry = ProjectileConfiguration.Projectiles[magicProjectileId];
    }
}
