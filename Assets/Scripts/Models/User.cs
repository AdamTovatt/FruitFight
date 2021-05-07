using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class User
{
    public bool IsLocal { get; set; }
    public PlayerInput ConnectedInput { get; set; }

    public User(bool isLocal)
    {
        IsLocal = isLocal;
    }
}
