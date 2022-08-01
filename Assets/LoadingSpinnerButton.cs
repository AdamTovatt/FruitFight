using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadingSpinnerButton : MonoBehaviour
{
    public Image LoadingSpinner;
    public TextMeshProUGUI ButtonText;

    public Button Button { get { if (_button == null) _button = gameObject.GetComponent<Button>(); return _button; } }
    private Button _button;

    public void ShowSpinner()
    {
        LoadingSpinner.gameObject.SetActive(true);
        ButtonText.gameObject.SetActive(false);
    }

    public void ReturnToNormal()
    {
        LoadingSpinner.gameObject.SetActive(false);
        ButtonText.gameObject.SetActive(true);
    }
}
