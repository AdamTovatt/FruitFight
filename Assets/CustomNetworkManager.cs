using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public static bool IsOnlineSession { get; set; }

    public static CustomNetworkManager Instance { get; private set; }

    public bool IsServer { get; set; }

    public delegate void DisconnectedHandlerClient(int id);
    public event DisconnectedHandlerClient OnDisconnectedClient;

    public delegate void DisconnectedHandlerServer(int id);
    public event DisconnectedHandlerServer OnDisconnectedServer;

    public delegate void ConnectedHandlerClient(int id);
    public event ConnectedHandlerClient OnConnectedClient;

    public delegate void ConnectedHandlerServer(int id);
    public event ConnectedHandlerServer OnConnectedServer;

    public override void Start()
    {
        base.Start();

        Instance = this;
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnDisconnectedClient?.Invoke(conn.connectionId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnConnectedClient?.Invoke(conn.connectionId);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        OnDisconnectedServer?.Invoke(conn.connectionId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        OnConnectedServer?.Invoke(conn.connectionId);

        if (conn.connectionId != 0)
        {
            AlertCreator.Instance.CreateNotification("Client connected: " + conn.connectionId);
        }
    }
}
