using System;
using UnityEngine;

/// <summary>
/// Current gameplay state
/// </summary>
public enum GameState
{
    Playing,
    Paused,
    Victory,
    GameOver
}

/// <summary>
/// Represents which gameplay phase the player is currently in (Farm or Combat)
/// </summary>
public enum GamePhase
{
    Farm,
    Combat
}

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; } = GameState.Playing;
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Farm;

    public event Action<GameState> OnStateChanged;
    public event Action<GamePhase> OnPhaseChanged;

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }

    public void SetGamePhase(GamePhase newPhase)
    {
        if (CurrentPhase == newPhase)
        {
            return;
        }
        CurrentPhase = newPhase;
        OnPhaseChanged?.Invoke(CurrentPhase);
    }
}