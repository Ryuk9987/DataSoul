using Godot;

public partial class DungeonDoor : StaticBody3D
{
    [Export] public bool IsLocked { get; set; } = true;

    /// <summary>
    /// Wenn gesetzt: Spieler wird beim Öffnen zu dieser Zone teleportiert.
    /// Wert = Node-Name der Zielzone (z.B. "Zone2", "SecretRoom").
    /// Leer lassen wenn die Tür nur aufgeht ohne Transition.
    /// </summary>
    [Export] public string TargetZoneName { get; set; } = "";

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        var area = GetNodeOrNull<Area3D>("InteractArea");
        if (area != null)
            area.BodyEntered += OnPlayerEnter;
    }

    private void OnPlayerEnter(Node3D body)
    {
        if (body is PlayerController)
        {
            if (!IsLocked)
            {
                if (!string.IsNullOrEmpty(TargetZoneName))
                    TriggerTransition();
                // Keine Message wenn offen aber kein Target (rein dekorative Tür)
            }
            else
            {
                ShowLockedMessage();
            }
        }
    }

    public void Open()
    {
        bool wasLocked = IsLocked;
        IsLocked = false;
        if (_animationPlayer != null && _animationPlayer.HasAnimation("open"))
        {
            _animationPlayer.Play("open");
        }
        else if (wasLocked)
        {
            // War gesperrt → jetzt öffnen (visuell verschwinden)
            Hide();
        }
        // War schon offen (Durchgang/Exit) → sichtbar lassen
    }

    private void TriggerTransition()
    {
        if (!string.IsNullOrEmpty(TargetZoneName))
        {
            ZoneManager.Instance?.MovePlayerToZone(TargetZoneName);
        }
    }

    private void ShowLockedMessage()
    {
        var hud = GetNodeOrNull<CanvasLayer>("/root/PlayerHUD");
        if (hud == null) return;

        var label = new Label();
        label.Text = "Weg gesperrt";
        label.CustomMinimumSize = new Vector2(200, 50);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeFontOverride("font", GD.Load<Font>("res://assets/fonts/console.ttf"));
        label.AddThemeColorOverride("font_color", new Color(1.0f, 0.3f, 0.3f));
        hud.AddChild(label);

        GetTree().CreateTimer(2.0).Timeout += () => label.QueueFree();
    }
}
