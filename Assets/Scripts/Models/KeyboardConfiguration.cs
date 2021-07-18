using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyboardConfiguration
{
    public List<KeyboardButtonRow> ButtonRows;

    public static KeyboardConfiguration LoadFromConfiguration()
    {
        return JsonUtility.FromJson<KeyboardConfiguration>(WorldUtilities.LoadTextFile("Configuration/Keyboard_English"));
    }
}

[Serializable]
public class KeyboardButtonRow
{
    public List<KeyboardButtonConfiguration> Buttons;
}

[Serializable]
public class KeyboardButtonConfiguration
{
    public KeyboardButtonType ButtonType;
    public string Text;

    public float GetButtonWidth(float buttonSize, float fontSize)
    {
        return buttonSize + ((Text.Length - 1) * (fontSize / 2));
    }
}

public enum KeyboardButtonType
{
    Character, Enter, Backspace, Escape, Shift, CapsLock
}
