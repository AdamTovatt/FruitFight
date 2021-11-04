using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkCharacter : NetworkBehaviour
{
    [SyncVar]
    public uint NetId;

    public bool IsLocalPlayer { get; private set; }
    public SingleTargetCamera Camera { get; private set; }

    private bool playerMovementActive;
    private PlayerMovement playerMovement;
    private Rigidbody playerRigidbody;
    private uint playerNetId;

    public SingleTargetCamera CameraPrefab;

    private void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerRigidbody = gameObject.GetComponent<Rigidbody>();

        if (CustomNetworkManager.IsOnlineSession)
        {
            playerMovement.enabled = playerMovementActive;
        }
        else
        {
            gameObject.GetComponent<PlayerInput>().enabled = true;
        }
    }

    [ClientRpc]
    public void SetupPlayerNetworkCharacter(bool server)
    {
        IsLocalPlayer = CustomNetworkManager.Instance.IsServer == server;

        playerMovement.enabled = IsLocalPlayer;

        if (IsLocalPlayer)
        {
            gameObject.GetComponent<PlayerInput>().enabled = true;
            playerMovementActive = true;
            playerMovement.enabled = true;

            SingleTargetCamera camera = null;
            try
            {
                camera = GameManager.Instance.CameraManager.AddCamera(gameObject.transform, GameManager.Instance.LocalPlayerConfiguration?.Input);
                camera.SetViewType(CameraViewType.Full);
                WorldBuilder.Instance.AddPreviousWorldObjects(camera.gameObject);
                Camera = camera;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            playerMovement.InitializePlayerInput(GameManager.Instance.LocalPlayerConfiguration, camera);
        }
    }
}
