using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLobbyMenu : MonoBehaviour
{
    private delegate void RoomNameFetchedHandler(string name);
    private event RoomNameFetchedHandler OnRoomNameFetched;

    public static bool IsActive { get { return Instance != null && Instance.gameObject.activeSelf; } }
    public static MainMenuLobbyMenu Instance;

    public TextMeshProUGUI HostInformationText;
    public TextMeshProUGUI LobbyNameText;
    public TextMeshProUGUI ContinueButtonText;
    public TextMeshProUGUI TitleText;
    public Button BackButton;
    public Button ContinueButton;
    public CenterContentContainer PlayerContainer;
    public GameObject LoadingSpinner;

    public GameObject NetworkMethodCallerPrefab;

    public Color PositiveColor;
    public Color NeutralColor;

    public CenterContentContainer ButtonsContainer;

    public MainMenuConnectedMenu ConnectedMenu;

    public GameObject PlayerInLobbyPrefab;

    private MainMenuOnlineMenu previousMenu;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ContinueButton.gameObject.SetActive(true);
        BackButton.onClick.AddListener(Back);
        ContinueButton.onClick.AddListener(Continue);
    }

    private void DisableContinueButton()
    {
        //ContinueButton.interactable = false;
        ContinueButton.image.color = NeutralColor;
    }

    public void EnableContinueButton()
    {
        //ContinueButton.interactable = true;
        ContinueButton.image.color = PositiveColor;
        ContinueButton.Select();
    }

    public void FindExistingPlayerIdentities()
    {
        foreach (PlayerNetworkIdentity player in FindObjectsOfType<PlayerNetworkIdentity>())
        {
            player.AddSelfToMainMenuLobbyMenu();
        }

        if (playerObjects.Count > 1)
        {
            TitleText.text = CustomNetworkManager.HasAuthority ? "waiting for you to continue..." : "waiting for host to make a choice...";
        }
    }

    public void Show()
    {
        Show(false, null, null);
    }

    public void Show(bool shouldStartHost, string hostInformation, MainMenuOnlineMenu previousMenu)
    {
        if (previousMenu != null)
            this.previousMenu = previousMenu;

        this.previousMenu.gameObject.SetActive(false);

        if (ContinueButton.gameObject.activeSelf)
            ContinueButton.Select();
        else
            BackButton.Select();

        if (hostInformation != null)
            HostInformationText.text = hostInformation;

        BindEventListeners();

        LoadingSpinner.SetActive(true);
        LobbyNameText.gameObject.SetActive(false);

        if (shouldStartHost)
        {
            DisplayRoomName();
            CustomNetworkManager.IsOnlineSession = true;
            CustomNetworkManager.Instance.IsServer = true;
            CustomNetworkManager.Instance.StartHost();
        }

        if (playerObjects.Count < 2)
        {
            DisableContinueButton();
            TitleText.text = "waiting for players to join...";
        }
    }

    private void RoomNameFetched(string name)
    {
        LobbyNameText.text = name;
        Debug.Log("text: " + LobbyNameText.text);
    }

    private async Task DisplayRoomName()
    {
        string name = await ApiHelper.GetRoomName();
        LoadingSpinner.SetActive(false);
        Debug.Log("Got room name: " + name);
        OnRoomNameFetched?.Invoke(name);
        LobbyNameText.SetText(string.Format("lobby name: {0}", name));
        LobbyNameText.ForceMeshUpdate(true);
        LobbyNameText.gameObject.SetActive(true);
    }

    public void AddPlayer(int playerId, string playerName)
    {
        if (!playerObjects.ContainsKey(playerId))
        {
            GameObject playerObject = Instantiate(PlayerInLobbyPrefab, PlayerContainer.transform);

            playerObject.GetComponent<UiPlayerInLobby>().SetName(playerName);

            PlayerContainer.AddContent(playerObject.GetComponent<RectTransform>());
            PlayerContainer.CenterContent();
            playerObjects.Add(playerId, playerObject);
        }
        else
        {
            playerObjects[playerId].GetComponent<UiPlayerInLobby>().SetName(playerName);
        }

        if (CustomNetworkManager.Instance.IsServer)
        {
            Debug.Log("connections: " + NetworkServer.connections.Count);
            if (NetworkServer.connections.Count > 1)
                EnableContinueButton();
            else
                DisableContinueButton();
        }
        else
        {
            RemoveConnectButtonForClient();
        }

        if (playerObjects.Count == 2)
        {
            TitleText.text = CustomNetworkManager.HasAuthority ? "waiting for you to continue..." : "waiting for host to make a choice...";
        }
    }

    public void RemoveConnectButtonForClient()
    {
        if (!CustomNetworkManager.Instance.IsServer && ButtonsContainer.Content.Count > 1)
        {
            ContinueButton.gameObject.SetActive(false);
            ButtonsContainer.Content = new List<RectTransform>() { BackButton.GetComponent<RectTransform>() };
            ButtonsContainer.CenterContent();
        }
    }

    private void PlayerDisconnected(int playerId)
    {
        RemovePlayer(playerId);

        if (NetworkServer.connections.Count < 2)
        {
            DisableContinueButton();
        }
    }

    private void BindEventListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedServer += PlayerDisconnected;
        OnRoomNameFetched += RoomNameFetched;
    }

    private void RemoveEventListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedServer -= PlayerDisconnected;
        OnRoomNameFetched -= RoomNameFetched;
    }

    private void Continue()
    {
        if (CustomNetworkManager.Instance.IsServer && NetworkServer.connections.Count > 1)
        {
            NetworkServer.Spawn(Instantiate(NetworkMethodCallerPrefab)); //create network method caller
            RemoveEventListeners();
            ConnectedMenu.gameObject.SetActive(true);
            ConnectedMenu.Show(this);
        }
        else
        {
            AlertCreator.Instance.CreateNotification("Another player needs to join first!", 3);
        }
    }

    public void RemovePlayer(int playerId)
    {
        if (playerObjects.ContainsKey(playerId))
        {
            GameObject playerObject = playerObjects[playerId];
            PlayerContainer.RemoveContent(playerObject.GetComponent<RectTransform>());
            PlayerContainer.CenterContent();
            Destroy(playerObject);

            playerObjects.Remove(playerId);
        }
        else
        {
            Debug.LogError("no playerobject exists: " + playerId);
        }
    }

    private void Back()
    {
        if (CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
            CustomNetworkManager.Instance.IsServer = false;
            CustomNetworkManager.IsOnlineSession = false;
        }

        RemoveEventListeners();

        List<int> playersToRemove = new List<int>();

        foreach (int playerId in playerObjects.Keys)
            playersToRemove.Add(playerId);
        foreach (int playerId in playersToRemove)
            RemovePlayer(playerId);

        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }
}
