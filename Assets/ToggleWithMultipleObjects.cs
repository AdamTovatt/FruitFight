using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleWithMultipleObjects : MonoBehaviour
{
    public List<GameObject> AdditionalObjects;

    private Toggle toggle;

    private void Awake()
    {
        toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((newValue) => ValueChanged(newValue));
        ValueChanged(toggle.isOn);
    }

    private void ValueChanged(bool newValue)
    {
        if(newValue)
        {
            foreach (GameObject gameObject in AdditionalObjects)
                gameObject.SetActive(true);
        }
        else
        {
            foreach (GameObject gameObject in AdditionalObjects)
                gameObject.SetActive(false);
        }
    }
}
