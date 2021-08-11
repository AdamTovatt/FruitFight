using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStart : MonoBehaviour
{
    public Button PlayButton;
    public Button LevelEditorButton;
    public Button BrowseLevelsButton;
    public Button OptionsButton;
    public Button ExitButton;

    private void OnBecameVisible()
    {
        Debug.Log("vI");
    }
}
