using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInactive : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
