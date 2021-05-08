using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfiguration
{
    public bool IsLocal { get; set; }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public int Hat { get; set; }

    public PlayerConfiguration(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            IsLocal = false;
        }
        else
        {
            Input = playerInput;
            PlayerIndex = playerInput.playerIndex;
            IsLocal = true;
        }
    }
}
