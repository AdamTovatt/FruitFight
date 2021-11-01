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

            if (!CustomNetworkManager.Instance.IsServer)
                SetName(name);
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
    private void SetName(string newName)
    {
        Name = newName;

        MainMenuLobbyMenu.Instance.RemovePlayer((int)netId);
        MainMenuLobbyMenu.Instance.AddPlayer((int)netId, Name);
    }

    [Command(requiresAuthority = false)]
    public void JoinPlayerOnServer()
    {
        Debug.Log("client joined");
    }
}
