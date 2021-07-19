using System;
using UnityEngine;

public class UiKeyboardInput : MonoBehaviour
{
    public GameObject KeyboardPrefab;

    private Keyboard currentKeyboard;

    public delegate void GotTextHandler(object sender, bool success, string text);
    public event GotTextHandler OnGotText;

    public string LastText { get; private set; }

    public void OpenKeyboard()
    {
        if(currentKeyboard != null)
            throw new Exception("A keyboard is already open");

        currentKeyboard = Instantiate(KeyboardPrefab, transform).GetComponent<Keyboard>();
        currentKeyboard.OnTextSubmitted += GotText;
        currentKeyboard.OnClosed += CloseKeyboard;
    }

    public void CloseKeyboard(object sender)
    {
        OnGotText?.Invoke(this, false, null);
        currentKeyboard.OnTextSubmitted -= GotText;
        currentKeyboard.OnClosed -= CloseKeyboard;
        Destroy(currentKeyboard.gameObject);
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
