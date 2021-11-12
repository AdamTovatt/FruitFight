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

    public delegate void OnAttackHandler(Vector3 position, MovingCharacter.AttackSide side);
    public OnAttackHandler OnAttack;

    public SingleTargetCamera CameraPrefab;
    public Player Player;

    private bool playerMovementActive;
    private PlayerMovement playerMovement;
    private Rigidbody playerRigidbody;
    private uint playerNetId;


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

            PlayerConfiguration playerConfiguration = PlayerConfigurationManager.Instance.PlayerConfigurations.ToArray().Where(x => x.Input != null).FirstOrDefault();

            SingleTargetCamera camera = null;
            try
            {
                camera = GameManager.Instance.CameraManager.AddCamera(gameObject.transform, playerConfiguration?.Input);
                camera.SetViewType(CameraViewType.Full);
                WorldBuilder.Instance.AddPreviousWorldObjects(camera.gameObject);
                Camera = camera;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            playerMovement.InitializePlayerInput(playerConfiguration, camera);
        }

        //create hat
        GameObject hatPrefab = PrefabKeeper.Instance.GetPrefab((IsLocalPlayer ? PlayerNetworkIdentity.LocalPlayerInstance.Hat : PlayerNetworkIdentity.OtherPlayerInstance.Hat).AsHatPrefabEnum());
        if (hatPrefab != null)
        {
            GameObject playerHat = Instantiate(hatPrefab, playerMovement.GetComponentInChildren<HatPoint>().transform.position, playerMovement.transform.rotation);
            playerHat.transform.SetParent(playerMovement.transform.GetComponentInChildren<HatPoint>().transform);
        }

        //create ui
        GameUi.Instance.CreatePlayerInfoUi(new PlayerInformation(null, playerMovement, playerMovement.gameObject.GetComponent<Health>()), IsLocalPlayer);
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

    public void SpawnAngel()
    {
        if(CustomNetworkManager.Instance.IsServer)
        {
            RpcSpawnAngel();
        }
        else
        {
            CmdSpawnAngel();
        }
    }

    [ClientRpc]
    private void RpcSpawnAngel()
    {
        Instantiate(Player.AngelPrefab, transform.position, transform.rotation);
    }

    [Command]
    private void CmdSpawnAngel()
    {
        RpcSpawnAngel();
    }

    public void Punch(Vector3 position, MovingCharacter.AttackSide side)
    {
        if(CustomNetworkManager.Instance.IsServer)
        {
            RpcPunch(position, (int)side);
        }
        else
        {
            CmdPunch(position, (int)side);
        }
    }

    [ClientRpc]
    private void RpcPunch(Vector3 position, int side)
    {
        if (!CustomNetworkManager.Instance.IsServer)
            OnAttack?.Invoke(position, (MovingCharacter.AttackSide)side);
    }

    [Command]
    private void CmdPunch(Vector3 position, int side)
    {
        OnAttack?.Invoke(position, (MovingCharacter.AttackSide)side);
    }
}
