using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PlayerMovement> Players;
    public Camera Camera;
    public bool IsDebug = false;
    private bool oldIsDebug = false;

    public delegate void IsDebugChangedHandler(object sender, bool newState);
    public event IsDebugChangedHandler OnDebugStateChanged;

    public void Awake()
    {
        Instance = this;
        Players = new List<PlayerMovement>();
    }

    public void Update()
    {
        if(oldIsDebug != IsDebug)
        {
            OnDebugStateChanged?.Invoke(this, IsDebug);
        }

        oldIsDebug = IsDebug;
    }
}
