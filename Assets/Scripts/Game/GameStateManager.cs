using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager
{
    public static GameState State { get; set; } = GameState.MainMenu;

    public static void SetGameState(GameState gameState)
    {
        State = gameState;
    }
}

public enum GameState
{
    Story, Free, MainMenu
}
