using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SyncVar]
    public int JellyBeans;

    [SyncVar]
    public int Coins;

    public delegate void JellyBeansUpdatedHandler(int newAmount);
    public event JellyBeansUpdatedHandler OnJellyBeansUpdated;

    public delegate void CoinsUpdatedHandler(int newAmount);
    public event CoinsUpdatedHandler OnCoinsUpdated;

    private void Start()
    {
        InvokeJellyBeansUpdated(JellyBeans);
        InvokeCoinsUpdated(Coins);
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
