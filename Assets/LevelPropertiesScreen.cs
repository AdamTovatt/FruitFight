using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPropertiesScreen : MonoBehaviour
{
    public Button CloseButton;

    private void Start()
    {
        CloseButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        WorldEditorUi.Instance.CloseLevelProperties();
    }

    public void Show()
    {
        TouchScreenKeyboard.Open("text", TouchScreenKeyboardType.Default);
    }
}
