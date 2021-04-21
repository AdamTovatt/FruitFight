using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PlayerMovement> Players;
    public Camera Camera;

    public void Awake()
    {
        Instance = this;
        Players = new List<PlayerMovement>();
    }
}
