using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLobbyMenu : MonoBehaviour
{
    public static bool IsActive { get { return Instance != null && Instance.gameObject.activeSelf; } }
    public static MainMenuLobbyMenu Instance;

    public TextMeshProUGUI HostInformationText;
    public TextMeshProUGUI ContinueButtonText;
    public Button BackButton;
    public Button ContinueButton;
    public CenterContentContainer PlayerContainer;

    public GameObject PlayerInLobbyPrefab;

    private MainMenuOnlineMenu previousMenu;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

    private Color continueButtonTextOriginalColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BackButton.onClick.AddListener(Back);
        ContinueButton.onClick.AddListener(Continue);

        continueButtonTextOriginalColor = ContinueButtonText.color;
        DisableContinueButton();
    }

    private void DisableContinueButton()
    {
        ContinueButton.interactable = false;
        ContinueButtonText.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    }

    private void EnableContinueButton()
    {
        ContinueButton.interactable = true;
        ContinueButtonText.color = continueButtonTextOriginalColor;
    }

    public void Show(bool shouldStartHost, string hostInformation, MainMenuOnlineMenu previousMenu)
    {
        if (previousMenu != null)
            this.previousMenu = previousMenu;

        this.previousMenu.gameObject.SetActive(false);
        BackButton.Select();
        HostInformationText.text = hostInformation;

        BindEventListeners();

        if (shouldStartHost)
        {
            CustomNetworkManager.Instance.IsServer = true;
            CustomNetworkManager.Instance.StartHost();
        }
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
    }

    private void PlayerConnected(int playerId)
    {
        /*
        GameObject playerObject = Instantiate(PlayerInLobbyPrefab, PlayerContainer.transform);
        PlayerContainer.AddContent(playerObject.GetComponent<RectTransform>());
        PlayerContainer.CenterContent();
        playerObjects.Add(playerId, playerObject);*/
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

        CustomNetworkManager.Instance.OnConnectedServer += PlayerConnected;
    }

    private void RemoveEventListeners()
    {
        CustomNetworkManager.Instance.OnDisconnectedServer -= PlayerDisconnected;

        CustomNetworkManager.Instance.OnConnectedServer -= PlayerConnected;
    }

    private void Continue()
    {
        Debug.Log("Continue from lobby *le troll face*");
        RemoveEventListeners();
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
