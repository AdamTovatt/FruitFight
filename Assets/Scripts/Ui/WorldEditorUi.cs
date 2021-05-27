using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditorUi : MonoBehaviour
{
    public GameObject PauseMenu;

    public void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
    }
}
