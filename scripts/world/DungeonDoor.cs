using Godot;

public partial class DungeonDoor : StaticBody3D
{
    [Export]
    public bool IsLocked { get; set; } = true;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        BodyEntered += OnPlayerEnter;
    }

    private void OnPlayerEnter(Node body)
    {
        if (body is PlayerController player && !IsLocked)
        {
            Open();
        }
        else if (body is PlayerController player && IsLocked)
        {
            ShowLockedMessage();
        }
    }

    public void Open()
    {
        IsLocked = false;
        _animationPlayer.Play("open");
    }

    private void ShowLockedMessage()
    {
        // Show locked message
        var hud = GetNode<CanvasLayer>("/root/PlayerHUD");
        var label = new Label();
        label.Text = "Weg gesperrt";
        label.RectMinSize = new Vector2(200, 50);
        label.Align = Label.AlignEnum.Center;
        label.Valign = Label.VAlign.Center;
        label.AddThemeFontOverride("font", GD.Load<Font>("res://assets/fonts/console.ttf"));
        label.AddThemeColorOverride("font_color", new Color(1.0f, 0.3f, 0.3f));
        hud.AddChild(label);

        // Remove the message after 2 seconds
        GetTree().CreateTimer(2.0).Timeout += () => label.QueueFree();
    }
}