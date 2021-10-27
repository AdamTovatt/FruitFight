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
    public Button BackButton;
    public Button ContinueButton;
    public CenterContentContainer PlayerContainer;

    public GameObject PlayerInLobbyPrefab;

    private MainMenuOnlineMenu previousMenu;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BackButton.onClick.AddListener(Back);
        ContinueButton.onClick.AddListener(Continue);

        ContinueButton.interactable = false;
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
            CustomNetworkManager.Instance.StartHost();
        }
    }

    public void AddPlayer(int playerId, string playerName)
    {
        GameObject playerObject = Instantiate(PlayerInLobbyPrefab, PlayerContainer.transform);

        playerObject.GetComponent<UiPlayerInLobby>().SetName(playerName);

        PlayerContainer.AddContent(playerObject.GetComponent<RectTransform>());
        PlayerContainer.CenterContent();
        playerObjects.Add(playerId, playerObject);
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
        GameObject playerObject = playerObjects[playerId];
        PlayerContainer.RemoveContent(playerObject.GetComponent<RectTransform>());
        PlayerContainer.CenterContent();
        Destroy(playerObject);

        playerObjects.Remove(playerId);
    }

    private void Back()
    {
        if (CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
        }

        RemoveEventListeners();

        List<int> playersToRemove = new List<int>();

        foreach (int playerId in playerObjects.Keys)
            playersToRemove.Add(playerId);
        foreach(int playerId in playersToRemove)
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
