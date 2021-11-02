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

    public bool IsLocalPlayer { get { return isLocalPlayer; } }

    [SyncVar]
    public string Name;
    [SyncVar]
    public uint NetId;
    [SyncVar]
    public int Hat;

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
            MainMenuLobbyMenu.Instance.AddPlayer((int)netId, Name);
        }
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
}
