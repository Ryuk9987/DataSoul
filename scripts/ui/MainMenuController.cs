using Godot;

/// <summary>
/// Hauptmenü-Controller für DataSoul.
/// Verwaltet alle Menü-Buttons und Overlay-Panels.
/// </summary>
public partial class MainMenuController : Control
{
    // --- Buttons ---
    private Button _continueButton;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _settingsButton;
    private Button _creditsButton;
    private Button _quitButton;

    // --- Panels/Overlays ---
    private Control _settingsPanel;
    private Control _creditsPanel;
    private Control _notImplementedDialog;

    // --- Settings Controls ---
    private HSlider _volumeSlider;
    private CheckButton _fullscreenToggle;

    // --- Partikel-Animation ---
    private Node _particleLayer;
    private float _particleTimer = 0f;

    // --- Konstanten ---
    private const string SaveGamePath = "user://savegame.dat";
    private const string CharacterCreationScene = "res://scenes/ui/CharacterCreation.tscn";

    public override void _Ready()
    {
        // Buttons
        _continueButton = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/ContinueButton");
        _newGameButton  = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/NewGameButton");
        _loadGameButton = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/LoadGameButton");
        _settingsButton = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/SettingsButton");
        _creditsButton  = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/CreditsButton");
        _quitButton     = GetNode<Button>("Background/CenterLayout/MenuContainer/ButtonList/QuitButton");

        // Panels
        _settingsPanel        = GetNode<Control>("SettingsPanel");
        _creditsPanel         = GetNode<Control>("CreditsPanel");
        _notImplementedDialog = GetNode<Control>("NotImplementedDialog");

        // Settings Controls
        _volumeSlider      = GetNode<HSlider>("SettingsPanel/PanelContent/VolumeSlider");
        _fullscreenToggle  = GetNode<CheckButton>("SettingsPanel/PanelContent/FullscreenToggle");

        // Partikel-Node
        _particleLayer = GetNode<Node>("ParticleLayer");

        // Signal-Verbindungen
        _continueButton.Pressed += OnContinuePressed;
        _newGameButton.Pressed  += OnNewGamePressed;
        _loadGameButton.Pressed += OnLoadGamePressed;
        _settingsButton.Pressed += OnSettingsPressed;
        _creditsButton.Pressed  += OnCreditsPressed;
        _quitButton.Pressed     += OnQuitPressed;

        // Schließen-Buttons
        GetNode<Button>("SettingsPanel/PanelContent/CloseButton").Pressed         += () => HideAllPanels();
        GetNode<Button>("CreditsPanel/PanelContent/CloseButton").Pressed          += () => HideAllPanels();
        GetNode<Button>("NotImplementedDialog/DialogContent/CloseButton").Pressed += () => HideAllPanels();

        // Settings-Logik
        _volumeSlider.ValueChanged    += OnVolumeChanged;
        _fullscreenToggle.Toggled     += OnFullscreenToggled;

        // Alle Panels verstecken
        HideAllPanels();

        // Speicherstand prüfen → Fortsetzen aktivieren
        _continueButton.Disabled = !FileAccess.FileExists(SaveGamePath);
        if (_continueButton.Disabled)
            _continueButton.Modulate = new Color(0.4f, 0.4f, 0.4f, 0.6f);

        // Aktuellen Vollbild-Status spiegeln
        _fullscreenToggle.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
    }

    public override void _Process(double delta)
    {
        // Partikel-Update wird vom GpuParticles2D-Node selbst gehandhabt
        // Hier könnten wir andere Animationen steuern
    }

    // =========================================================
    // Button-Handler
    // =========================================================

    private void OnContinuePressed()
    {
        if (!FileAccess.FileExists(SaveGamePath)) return;
        // TODO: Savegame laden und in die Spielwelt wechseln
        GD.Print("Fortsetzen: Savegame vorhanden — Laden noch nicht implementiert.");
    }

    private void OnNewGamePressed()
    {
        GetTree().ChangeSceneToFile(CharacterCreationScene);
    }

    private void OnLoadGamePressed()
    {
        HideAllPanels();
        _notImplementedDialog.Visible = true;
    }

    private void OnSettingsPressed()
    {
        bool wasVisible = _settingsPanel.Visible;
        HideAllPanels();
        _settingsPanel.Visible = !wasVisible;
    }

    private void OnCreditsPressed()
    {
        bool wasVisible = _creditsPanel.Visible;
        HideAllPanels();
        _creditsPanel.Visible = !wasVisible;
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }

    // =========================================================
    // Settings-Handler
    // =========================================================

    private void OnVolumeChanged(double value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), (float)GD.Linear2Db((float)value));
    }

    private void OnFullscreenToggled(bool pressed)
    {
        DisplayServer.WindowSetMode(
            pressed
                ? DisplayServer.WindowMode.Fullscreen
                : DisplayServer.WindowMode.Windowed
        );
    }

    // =========================================================
    // Hilfsmethoden
    // =========================================================

    private void HideAllPanels()
    {
        _settingsPanel.Visible        = false;
        _creditsPanel.Visible         = false;
        _notImplementedDialog.Visible = false;
    }
}
