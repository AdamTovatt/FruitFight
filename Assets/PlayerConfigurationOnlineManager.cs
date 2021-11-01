using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfigurationOnlineManager : NetworkBehaviour
{
    public PlayerConfigurationManager Manager;

    [Command(requiresAuthority = false)]
    public void JoinPlayerOnServer()
    {
        Debug.Log("client joined");
    }
}
