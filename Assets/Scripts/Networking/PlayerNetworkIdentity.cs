using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkIdentity : NetworkBehaviour
{
    public static PlayerNetworkIdentity LocalPlayerInstance;
    public static PlayerNetworkIdentity OtherPlayerInstance { get { if (otherPlayerInstance == null) otherPlayerInstance = GetOtherPlayerInstance(); return otherPlayerInstance; } }

    private static PlayerNetworkIdentity otherPlayerInstance;

    public delegate void ReadyStatusUpdatedHandler(bool isLocalPlayer, bool newValue);
    public ReadyStatusUpdatedHandler OnReadyStatusUpdated;

    public bool IsLocalPlayer { get { return isLocalPlayer; } }

    [SyncVar]
    public string Name;
    [SyncVar]
    public uint NetId;
    [SyncVar]
    public int Hat;
    [SyncVar]
    public bool Ready;

    public Texture2D Portrait { get; set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
        {
            LocalPlayerInstance = this;
            string name = "Unknown user";

            if (ApiHelper.UserCredentials != null)
            {
                name = ApiHelper.UserCredentials.Username + DateTime.Now.Second.ToString();
            }

            Name = name;
            NetId = netId;

            if (!CustomNetworkManager.Instance.IsServer)
                SetName(name, netId);
        }

        if (MainMenuLobbyMenu.IsActive)
        {
            AddSelfToMainMenuLobbyMenu();
        }
    }

    public void AddSelfToMainMenuLobbyMenu()
    {
        MainMenuLobbyMenu.Instance.AddPlayer((int)netId, Name);
    }

    private static PlayerNetworkIdentity GetOtherPlayerInstance()
    {
        foreach(PlayerNetworkIdentity identity in FindObjectsOfType<PlayerNetworkIdentity>())
        {
            if (!identity.IsLocalPlayer)
                return identity;
        }

        return null;
    }

    private void OnDestroy()
    {
        if (MainMenuLobbyMenu.IsActive)
        {
            MainMenuLobbyMenu.Instance.RemovePlayer((int)netId);
        }
    }

    public void SetHat(int hat)
    {
        if(CustomNetworkManager.Instance.IsServer)
        {
            Hat = hat;
        }
        else
        {
            CmdSetHat(hat);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetHat(int hat)
    {
        Hat = hat;
    }

    [Command]
    private void SetName(string newName, uint newNetId)
    {
        Name = newName;
        NetId = newNetId;

        MainMenuLobbyMenu.Instance.RemovePlayer((int)netId);
        MainMenuLobbyMenu.Instance.AddPlayer((int)netId, Name);
    }

    [Command(requiresAuthority = false)]
    public void JoinPlayerOnServer()
    {
        PlayerConfigurationManager.Instance.ClientJoinSetupScreen();
    }

    [ClientRpc]
    private void RpcSetReady(bool newValue)
    {
        Ready = newValue;
        OnReadyStatusUpdated?.Invoke(IsLocalPlayer, newValue);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetReady(bool newValue)
    {
        RpcSetReady(newValue);
    }

    public void SetReady(bool newValue)
    {
        if(CustomNetworkManager.Instance.IsServer)
        {
            RpcSetReady(newValue);
        }
        else
        {
            CmdSetReady(newValue);
        }
    }
}
