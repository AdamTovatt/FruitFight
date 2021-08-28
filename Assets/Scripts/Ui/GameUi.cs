using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(AlertCreator))]
public class GameUi : MonoBehaviour
{
    public GameObject PlayerInfoUiPrefab;
    public Canvas Canvas;
    public InputSystemUIInputModule UiInput;
    public EventSystem EventSystem;

    public static GameUi Instance { get; private set; }

    private List<UiPlayerInfo> playerInfos;

    private void Awake()
    {
        playerInfos = new List<UiPlayerInfo>();
        Instance = this;
        AlertCreator.SetInstance(gameObject.GetComponent<AlertCreator>());
    }

    private void Start()
    {
        UiInput.enabled = false;

        if (WorldEditorUi.Instance != null) //the world editor has it's own input system that will take over if we come from the editor
        {
            EventSystem.enabled = false;
        }
    }

    public void CreatePlayerInfoUi(PlayerInformation playerInformation)
    {
        UiPlayerInfo uiPlayerInfo = Instantiate(PlayerInfoUiPrefab, Canvas.transform).GetComponent<UiPlayerInfo>();
        uiPlayerInfo.Init(playerInformation, !(playerInfos.Count > 0)); //if it's the first player we will set it to be left
        playerInfos.Add(uiPlayerInfo);
    }
}
