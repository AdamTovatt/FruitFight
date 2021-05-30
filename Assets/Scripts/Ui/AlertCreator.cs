using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCreator : MonoBehaviour
{
    public GameObject AlertPrefab;

    public Alert CreateAlert(string text, List<string> buttons)
    {
        Alert alert = Instantiate(AlertPrefab, transform).GetComponent<Alert>();
        alert.Initialize(text, buttons);
        return alert;
    }
}
