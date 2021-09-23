using System;
using UnityEngine;

public class UiKeyboardInput : MonoBehaviour
{
    public GameObject KeyboardPrefab;

    private Keyboard currentKeyboard;
    private GameObject currentKeyboardContainer;

    public delegate void GotTextHandler(object sender, bool success, string text);
    public event GotTextHandler OnGotText;

    public string LastText { get; private set; }

    public void OpenKeyboard()
    {
        if(currentKeyboard != null)
            throw new Exception("A keyboard is already open");

        currentKeyboardContainer = Instantiate(KeyboardPrefab, transform);
        currentKeyboard = currentKeyboardContainer.GetComponentInChildren<Keyboard>();
        currentKeyboard.OnTextSubmitted += GotText;
        currentKeyboard.OnClosed += CloseKeyboard;
    }

    public void CloseKeyboard(object sender)
    {
        OnGotText?.Invoke(this, false, null);
        currentKeyboard.OnTextSubmitted -= GotText;
        currentKeyboard.OnClosed -= CloseKeyboard;
        Destroy(currentKeyboardContainer);
        currentKeyboard = null;
    }

    public void GotText(object sender, string text)
    {
        LastText = text;
        OnGotText?.Invoke(this, true, text);
        OnGotText = null;
        CloseKeyboard(this);
    }
}
