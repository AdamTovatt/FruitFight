using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public WorldBuilder Builder { get; private set; }
    public World World { get; private set; }

    public void Awake()
    {
        World = new World();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
