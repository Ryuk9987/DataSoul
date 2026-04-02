using Godot;
using System;

public partial class CharacterCreationController : Control
{
    [Export] private LineEdit _nameLineEdit;
    [Export] private Button _programmerButton;
    [Export] private Button _gamerButton;
    [Export] private Button _hackerButton;
    [Export] private Button _creatorButton;
    [Export] private Button _analystButton;
    [Export] private Label _infoLabel;
    [Export] private Button _startButton;
    
    private PlayerStats.Background _selectedBackground = PlayerStats.Background.Gamer;
    private bool _backgroundSelected = false;
    
    public override void _Ready()
    {
        _programmerButton.Pressed += () => SelectBackground(PlayerStats.Background.Programmer);
        _gamerButton.Pressed += () => SelectBackground(PlayerStats.Background.Gamer);
        _hackerButton.Pressed += () => SelectBackground(PlayerStats.Background.Hacker);
        _creatorButton.Pressed += () => SelectBackground(PlayerStats.Background.Creator);
        _analystButton.Pressed += () => SelectBackground(PlayerStats.Background.Analyst);
        
        _startButton.Pressed += StartGame;
    }
    
    private void SelectBackground(PlayerStats.Background background)
    {
        _selectedBackground = background;
        _backgroundSelected = true;
        UpdateInfoLabel();
    }
    
    private void UpdateInfoLabel()
    {
        string infoText = "";
        
        switch (_selectedBackground)
        {
            case PlayerStats.Background.Programmer:
                infoText = "Code-Injection, +3 ATK +2 SPD";
                break;
            case PlayerStats.Background.Gamer:
                infoText = "Combo-Rush, +5 ATK";
                break;
            case PlayerStats.Background.Hacker:
                infoText = "System-Exploit, +4 SPD";
                break;
            case PlayerStats.Background.Creator:
                infoText = "Illusion-Field, +10 HP";
                break;
            case PlayerStats.Background.Analyst:
                infoText = "Weakness-Scan, +3 DEF";
                break;
        }
        
        _infoLabel.Text = infoText;
    }
    
    private void StartGame()
    {
        if (string.IsNullOrEmpty(_nameLineEdit.Text))
        {
            GD.Print("Bitte gib einen Namen ein.");
            return;
        }
        
        if (!_backgroundSelected)
        {
            GD.Print("Bitte wähle einen Background aus.");
            return;
        }
        
        // Speichere die Charakterdaten im GameSession-Singleton
        GameSession.PlayerName = _nameLineEdit.Text;
        GameSession.PlayerBackground = _selectedBackground;
        GameSession.IsInitialized = true;

        // Lade die nächste Szene
        GetTree().ChangeSceneToFile("res://scenes/world/FirewallRuins/FirewallRuins.tscn");
    }
}