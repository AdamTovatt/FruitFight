using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerConfiguration
{
    public bool IsLocal { get; set; }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public int Hat { get; set; }
    public Texture2D Portrait { get; set; }

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

    public Prefab? GetHatAsPrefabEnum()
    {
        if (Hat == 1)
            return Prefab.WizardHat;

        if (Hat == 2)
            return Prefab.Beanie;

        if (Hat == 3)
            return Prefab.SweatBand;

        return null;
    }
}
