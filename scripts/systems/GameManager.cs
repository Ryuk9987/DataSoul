using Godot;
using System;

public partial class GameManager : Node
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    [Export] public PlayerStats PlayerStats;
    [Export] public FragmentSystem FragmentSystem;
    [Export] public SkillSystem SkillSystem;

    public override void _Ready()
    {
        // Initialize systems
        CurrentState = GameState.Playing;
    }

    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        GetTree().Paused = true;
    }

    public void ResumeGame()
    {
        CurrentState = GameState.Playing;
        GetTree().Paused = false;
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        // Handle game over logic
    }
}