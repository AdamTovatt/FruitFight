using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public bool StartActive = false;

    private void Start()
    {
        gameObject.SetActive(StartActive);
    }

    public void WasShown()
    {
        ResumeButton.Select();
    }
}
