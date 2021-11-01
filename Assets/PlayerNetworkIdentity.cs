using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkIdentity : NetworkBehaviour
{
    public static PlayerNetworkIdentity LocalPlayerInstance;

    [SyncVar]
    public string Name;
    [SyncVar]
    public uint NetId;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
        {
            LocalPlayerInstance = this;
            string name = "Unknown user";

            if (ApiHelper.UserCredentials != null)
            {
                name = ApiHelper.UserCredentials.Username + DateTime.Now.ToString();
            }

            Name = name;
            NetId = netId;

            if (!CustomNetworkManager.Instance.IsServer)
                SetName(name, netId);
        }

        if (MainMenuLobbyMenu.IsActive)
        {
            Debug.Log("add player: " + Name);
            MainMenuLobbyMenu.Instance.AddPlayer((int)netId, Name);
        }
    }

    private void OnDestroy()
    {
        if (MainMenuLobbyMenu.IsActive)
        {
            MainMenuLobbyMenu.Instance.RemovePlayer((int)netId);
        }
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
