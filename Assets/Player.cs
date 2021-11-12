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

    public delegate void JellyBeansUpdatedHandler(int newAmount);
    public event JellyBeansUpdatedHandler OnJellyBeansUpdated;

    public delegate void CoinsUpdatedHandler(int newAmount);
    public event CoinsUpdatedHandler OnCoinsUpdated;

    public GameObject BeamOfLightPrefab;
    public GameObject AngelPrefab;

    public TemporaryMeshReplacer MeshReplacer;
    public Health Health;
    public PlayerMovement Movement;
    public PlayerNetworkCharacter NetworkCharacter;

    private List<float> previousDeathTimes = new List<float>();

    private void Start()
    {
        InvokeJellyBeansUpdated(JellyBeans);
        InvokeCoinsUpdated(Coins);

        Health.OnDied += OnDied;
    }

    private void OnDestroy()
    {
        Health.OnDied -= OnDied;
    }

    private void OnDied(Health sender, CauseOfDeath causeOfDeath)
    {
        if (!CustomNetworkManager.IsOnlineSession || NetworkCharacter.IsLocalPlayer)
        {
            if (causeOfDeath == CauseOfDeath.Water)
            {
                previousDeathTimes.Add(Time.time);
                if (previousDeathTimes.Count > 5)
                    previousDeathTimes.RemoveAt(0);

                if (previousDeathTimes.Count >= 5 && previousDeathTimes.Max() - previousDeathTimes.Min() < 10) //if we have died 5 times within 10 seconds
                {
                    if (Movement.PreviousGroundedPositions.Keys.Count > 1)
                    {
                        Movement.PreviousGroundedPositions.Remove(Movement.PreviousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key);
                        previousDeathTimes.Clear();
                    }

                    transform.position = Movement.PreviousGroundedPositions[Movement.PreviousGroundedPositions.OrderByDescending(x => x.Value.Time).First().Key].Position;
                }
                else
                {
                    transform.position = Movement.LastGroundedPosition.Position;
                }

                Health.CanDie = true;
                Health.IsDead = false;
            }
            else
            {
                Movement.ControlsEnabled = false;

                if (!CustomNetworkManager.IsOnlineSession)
                {
                    Instantiate(AngelPrefab, transform.position, transform.rotation);
                }
                else
                {
                    NetworkCharacter.SpawnAngel();
                }

                MeshReplacer.ReplaceMesh();

                this.CallWithDelay(Respawn, 3);
            }
        }
    }

    public void Respawn()
    {
        if (CustomNetworkManager.IsOnlineSession)
        {
            if (CustomNetworkManager.Instance.IsServer)
            {
                RpcRespawn();
            }
            else
            {
                CmdRespawn();
            }
        }
        else
        {
            PerformRespawn();
        }
    }

    [Command]
    private void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        PerformRespawn();
    }

    private void PerformRespawn()
    {
        BeamOfLight beamOfLight = Instantiate(BeamOfLightPrefab, transform.position, transform.rotation).GetComponent<BeamOfLight>();

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

    public void LostItem(AbsorbableItemType itemType)
    {
        switch (itemType)
        {
            case AbsorbableItemType.JellyBean:
                JellyBeans--;
                InvokeJellyBeansUpdated(JellyBeans);
                break;
            case AbsorbableItemType.Coin:
                Coins--;
                InvokeCoinsUpdated(Coins);
                break;
            default:
                break;
        }
    }

    public void InvokeJellyBeansUpdated(int jellyBeans)
    {
        if (CustomNetworkManager.IsOnlineSession)
            RpcInvokeJellyBeansUpdated(jellyBeans);
        else
            OnJellyBeansUpdated?.Invoke(jellyBeans);
    }

    public void InvokeCoinsUpdated(int coins)
    {
        if (CustomNetworkManager.IsOnlineSession)
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
}
