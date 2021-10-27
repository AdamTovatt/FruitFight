using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkIdentity : NetworkBehaviour
{
    [SyncVar]
    public string Name;

    private void Start()
    {
        if (isLocalPlayer)
        {

            string name = "Unknown user";

            if (ApiHelper.UserCredentials != null)
            {
                name = ApiHelper.UserCredentials.Username + DateTime.Now.ToString();
            }

            Name = name;
            SetName(name);
        }

        if(MainMenuLobbyMenu.IsActive)
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
}
