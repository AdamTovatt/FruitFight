using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLobbyMenu : MonoBehaviour
{
    public TextMeshProUGUI HostInformationText;
    public Button BackButton;
    public Button ContinueButton;
    public CenterContentContainer PlayerContainer;

    public GameObject PlayerInLobbyPrefab;

    private MainMenuOnlineMenu previousMenu;
    private Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();

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

    private void PlayerConnected(int playerId)
    {
        GameObject playerObject = Instantiate(PlayerInLobbyPrefab, PlayerContainer.transform);
        PlayerContainer.AddContent(playerObject.GetComponent<RectTransform>());
        PlayerContainer.CenterContent();
        playerObjects.Add(playerId, playerObject);
    }

    private void PlayerDisconnected(int playerId)
    {
        GameObject playerObject = playerObjects[playerId];
        PlayerContainer.RemoveContent(playerObject.GetComponent<RectTransform>());
        PlayerContainer.CenterContent();
        Destroy(playerObject);

        playerObjects.Remove(playerId);
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

    private void Back()
    {
        if (CustomNetworkManager.Instance.isNetworkActive)
        {
            CustomNetworkManager.Instance.StopClient();
            CustomNetworkManager.Instance.StopHost();
        }

        RemoveEventListeners();

        previousMenu.gameObject.SetActive(true);
        previousMenu.Show(null);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }
}
