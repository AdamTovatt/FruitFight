using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject PlayerSetupPanelPrefab;
    public PlayerInput Input;

    private void Awake()
    {
        GameObject rootMenu = GameObject.Find("MainLayout");
        if(rootMenu != null)
        {
            GameObject menu = Instantiate(PlayerSetupPanelPrefab, rootMenu.transform);
            Input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(Input, true);
            PlayerConfigurationManager.Instance.PlayerSetupMenuWasCreatedLocally(menu);
        }
    }
}
