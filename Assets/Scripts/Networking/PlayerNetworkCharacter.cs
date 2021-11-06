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

    public bool? IsStandingStill { get; private set; }

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

            if (CustomNetworkManager.Instance.IsServer)
            {
                gameObject.GetComponent<NetworkTransform>().clientAuthority = false;
            }

            SingleTargetCamera camera = null;
            try
            {
                camera = GameManager.Instance.CameraManager.AddCamera(gameObject.transform, GameManager.LocalPlayerConfiguration?.Input);
                camera.SetViewType(CameraViewType.Full);
                WorldBuilder.Instance.AddPreviousWorldObjects(camera.gameObject);
                Camera = camera;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            playerMovement.InitializePlayerInput(GameManager.LocalPlayerConfiguration, camera);
        }

        GameObject hatPrefab = PrefabKeeper.Instance.GetPrefab((IsLocalPlayer ? PlayerNetworkIdentity.LocalPlayerInstance.Hat : PlayerNetworkIdentity.OtherPlayerInstance.Hat).AsHatPrefabEnum());
        if (hatPrefab != null)
        {
            GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
            playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
        }
    }

    public void SetStandingStill(bool newValue)
    {
        if(CustomNetworkManager.Instance.IsServer)
        {
            RpcSetStandingStill(newValue);
        }
        else
        {
            CommandSetStandingStill(newValue);
        }
    }

    [ClientRpc]
    private void RpcSetStandingStill(bool newValue)
    {
        IsStandingStill = newValue;
    }

    [Command]
    private void CommandSetStandingStill(bool newValue)
    {
        IsStandingStill = newValue;
    }
}
