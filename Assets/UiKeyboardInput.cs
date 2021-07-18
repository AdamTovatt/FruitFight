using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiKeyboardInput : MonoBehaviour
{
    public GameObject KeyboardPrefab;

    public void OpenKeyboard()
    {
        GameObject keyboard = Instantiate(KeyboardPrefab, transform);
    }
}
