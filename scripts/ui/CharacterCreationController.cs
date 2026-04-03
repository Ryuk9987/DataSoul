using Godot;

public partial class CharacterCreationController : Control
{
    private LineEdit _nameLineEdit;
    private Button _programmerButton;
    private Button _gamerButton;
    private Button _hackerButton;
    private Button _creatorButton;
    private Button _analystButton;
    private Label _infoLabel;
    private Button _startButton;

    private PlayerStats.Background _selectedBackground = PlayerStats.Background.Gamer;
    private bool _backgroundSelected = false;

    public override void _Ready()
    {
        // Nodes per Pfad holen — kein Export/Inspector nötig
        _nameLineEdit      = GetNode<LineEdit>("CenterContainer/VBox/NameLineEdit");
        _programmerButton  = GetNode<Button>("CenterContainer/VBox/BackgroundGrid/ProgrammerButton");
        _gamerButton       = GetNode<Button>("CenterContainer/VBox/BackgroundGrid/GamerButton");
        _hackerButton      = GetNode<Button>("CenterContainer/VBox/BackgroundGrid/HackerButton");
        _creatorButton     = GetNode<Button>("CenterContainer/VBox/BackgroundGrid/CreatorButton");
        _analystButton     = GetNode<Button>("CenterContainer/VBox/BackgroundGrid/AnalystButton");
        _infoLabel         = GetNode<Label>("CenterContainer/VBox/InfoLabel");
        _startButton       = GetNode<Button>("CenterContainer/VBox/StartButton");

        _programmerButton.Pressed += () => SelectBackground(PlayerStats.Background.Programmer);
        _gamerButton.Pressed      += () => SelectBackground(PlayerStats.Background.Gamer);
        _hackerButton.Pressed     += () => SelectBackground(PlayerStats.Background.Hacker);
        _creatorButton.Pressed    += () => SelectBackground(PlayerStats.Background.Creator);
        _analystButton.Pressed    += () => SelectBackground(PlayerStats.Background.Analyst);
        _startButton.Pressed      += StartGame;
    }

    private void SelectBackground(PlayerStats.Background background)
    {
        _selectedBackground = background;
        _backgroundSelected = true;

        // Alle Buttons zurücksetzen
        foreach (var btn in new[] { _programmerButton, _gamerButton, _hackerButton, _creatorButton, _analystButton })
            btn.Modulate = new Color(1, 1, 1, 1);

        // Ausgewählten Button hervorheben
        GetButtonForBackground(background).Modulate = new Color(0.2f, 0.8f, 1.0f, 1f);

        UpdateInfoLabel();
    }

    private Button GetButtonForBackground(PlayerStats.Background bg) => bg switch
    {
        PlayerStats.Background.Programmer => _programmerButton,
        PlayerStats.Background.Gamer      => _gamerButton,
        PlayerStats.Background.Hacker     => _hackerButton,
        PlayerStats.Background.Creator    => _creatorButton,
        PlayerStats.Background.Analyst    => _analystButton,
        _ => _gamerButton
    };

    private void UpdateInfoLabel()
    {
        _infoLabel.Text = _selectedBackground switch
        {
            PlayerStats.Background.Programmer => "Start-Skill: Code-Injection\n+3 ATK  +2 SPD",
            PlayerStats.Background.Gamer      => "Start-Skill: Combo-Rush\n+5 ATK",
            PlayerStats.Background.Hacker     => "Start-Skill: System-Exploit\n+4 SPD",
            PlayerStats.Background.Creator    => "Start-Skill: Illusion-Field\n+10 HP",
            PlayerStats.Background.Analyst    => "Start-Skill: Weakness-Scan\n+3 DEF",
            _ => ""
        };
    }

    private void StartGame()
    {
        if (string.IsNullOrEmpty(_nameLineEdit.Text.Trim()))
        {
            _infoLabel.Text = "Bitte gib einen Namen ein!";
            _infoLabel.Modulate = new Color(1, 0.3f, 0.3f, 1);
            return;
        }

        if (!_backgroundSelected)
        {
            _infoLabel.Text = "Bitte wähle einen Background!";
            _infoLabel.Modulate = new Color(1, 0.3f, 0.3f, 1);
            return;
        }

        GameSession.PlayerName       = _nameLineEdit.Text.Trim();
        GameSession.PlayerBackground = _selectedBackground;
        GameSession.IsInitialized    = true;

        GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/Beschwoerungsraum.tscn");
    }
}
