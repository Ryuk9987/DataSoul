using Godot;

public partial class DataNode : StaticBody3D
{
    [Export] public string NodeId { get; set; } = "data_node_1";
    [Export] public string NodeLabel { get; set; } = "Data-Node";

    public static bool IsMenuOpen { get; private set; } = false;

    private bool _playerInRange = false;
    private bool _menuOpen = false;
    private float _pulseTimer = 0f;
    private MeshInstance3D _orb;

    // Menü lebt als globaler Node — nicht als Child von DataNode
    private static CanvasLayer _sharedMenu;
    private static DataNode _menuOwner;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        var area = GetNodeOrNull<Area3D>("InteractArea");
        if (area != null)
        {
            area.BodyEntered += OnBodyEntered;
            area.BodyExited  += OnBodyExited;
        }
        _orb = GetNodeOrNull<MeshInstance3D>("Orb");
    }

    public override void _Process(double delta)
    {
        if (_orb != null && !_menuOpen)
        {
            _pulseTimer += (float)delta * 2f;
            _orb.Scale = Vector3.One * (1f + 0.08f * Mathf.Sin(_pulseTimer));
        }

        if (_playerInRange && !_menuOpen && !GetTree().Paused
            && Input.IsActionJustPressed("interact"))
            OpenMenu();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_menuOpen && @event is InputEventKey key && key.Pressed
            && key.Keycode == Key.Escape)
        {
            CloseMenu();
            GetViewport().SetInputAsHandled();
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerController)
        {
            _playerInRange = true;
            SaveGame();
            DialogueSystem.Instance?.ShowLine("System",
                $"[ {NodeLabel} ] — Spielstand gespeichert. [F] für Optionen.", 2.5f);
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is PlayerController)
        {
            _playerInRange = false;
            if (_menuOpen) CloseMenu();
        }
    }

    private void OpenMenu()
    {
        _menuOpen = true;
        IsMenuOpen = true;
        _menuOwner = this;

        // Menü als Child von Root hinzufügen — unabhängig von Pause
        _sharedMenu = BuildMenu();
        GetTree().Root.AddChild(_sharedMenu);

        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void CloseMenu()
    {
        _menuOpen = false;
        IsMenuOpen = false;
        _menuOwner = null;

        _sharedMenu?.QueueFree();
        _sharedMenu = null;

        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private CanvasLayer BuildMenu()
    {
        var canvas = new CanvasLayer();
        canvas.Layer = 20;
        canvas.ProcessMode = ProcessModeEnum.Always;

        var panel = new Panel();
        panel.ProcessMode = ProcessModeEnum.Always;
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);
        panel.CustomMinimumSize = new Vector2(320, 260);
        panel.OffsetLeft = -160; panel.OffsetTop = -130;
        panel.OffsetRight = 160; panel.OffsetBottom = 130;

        var style = new StyleBoxFlat();
        style.BgColor = new Color(0.04f, 0.06f, 0.14f, 0.95f);
        style.BorderColor = new Color(0.1f, 0.6f, 1.0f, 0.9f);
        style.SetBorderWidthAll(2);
        style.SetCornerRadiusAll(8);
        panel.AddThemeStyleboxOverride("panel", style);
        canvas.AddChild(panel);

        var vbox = new VBoxContainer();
        vbox.ProcessMode = ProcessModeEnum.Always;
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        vbox.AddThemeConstantOverride("separation", 12);
        vbox.OffsetLeft = 20; vbox.OffsetTop = 16;
        vbox.OffsetRight = -20; vbox.OffsetBottom = -16;
        panel.AddChild(vbox);

        var title = new Label();
        title.ProcessMode = ProcessModeEnum.Always;
        title.Text = $"◈  {NodeLabel}";
        title.HorizontalAlignment = HorizontalAlignment.Center;
        title.AddThemeColorOverride("font_color", new Color(0.3f, 0.8f, 1.0f));
        title.AddThemeFontSizeOverride("font_size", 16);
        vbox.AddChild(title);

        vbox.AddChild(new HSeparator());

        AddBtn(vbox, "▶  Fortsetzen", CloseMenu);
        AddBtn(vbox, "⚡  Skills wechseln", () => {
            CloseMenu();
            DialogueSystem.Instance?.ShowLine("System", "Skill-Menü: [I] drücken.", 2f);
        });
        AddBtn(vbox, "↩  Zum letzten Data-Node", () => {
            CloseMenu();
            DialogueSystem.Instance?.ShowLine("System", "Fast-Travel folgt in Kürze.", 2f);
        });

        var hint = new Label();
        hint.ProcessMode = ProcessModeEnum.Always;
        hint.Text = "[ESC] Schließen";
        hint.HorizontalAlignment = HorizontalAlignment.Right;
        hint.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.5f));
        hint.AddThemeFontSizeOverride("font_size", 11);
        vbox.AddChild(hint);

        return canvas;
    }

    private void AddBtn(VBoxContainer parent, string text, System.Action onPressed)
    {
        var btn = new Button();
        btn.ProcessMode = ProcessModeEnum.Always;
        btn.Text = text;
        btn.CustomMinimumSize = new Vector2(0, 40);
        btn.AddThemeFontSizeOverride("font_size", 14);
        btn.Pressed += () => onPressed();
        parent.AddChild(btn);
    }

    private void SaveGame()
    {
        var config = new ConfigFile();
        var player = GetTree().Root.FindChild("Player", true, false) as Node3D;
        if (player != null)
            config.SetValue("save", "player_position", player.GlobalPosition);
        config.SetValue("save", "node_id", NodeId);

        var lvl = GetTree().Root.FindChild("LevelSystem", true, false) as LevelSystem;
        if (lvl != null)
        {
            config.SetValue("save", "level", lvl.Level);
            config.SetValue("save", "xp", lvl.CurrentXp);
        }
        config.Save("user://savegame.cfg");
        GD.Print($"[DataNode] Gespeichert bei {NodeId}");
    }
}
