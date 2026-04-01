using Godot;

public partial class DataCrystal : Area3D
{
    [Signal]
    public delegate void CrystalFoundEventHandler();

    public override void _Ready()
    {
        BodyEntered += OnPlayerEnter;
    }

    private void OnPlayerEnter(Node body)
    {
        if (body is PlayerController player)
        {
            ShowCrystalMessage();
            EmitSignal(nameof(CrystalFound));
        }
    }

    private void ShowCrystalMessage()
    {
        // Show the message overlay
        var message = "...ich weiß nicht wie lange ich noch... der Fluch hält. Ich kann nicht zurück. Ich kann nicht kämpfen. Wenn jemand das findet — The Root ist echt. Valdris lügt. Sucht Kael. Er weiß—";
        var hud = GetNode<CanvasLayer>("/root/PlayerHUD");
        var label = new Label();
        label.Text = message;
        label.RectMinSize = new Vector2(800, 200);
        label.Align = Label.AlignEnum.Center;
        label.Valign = Label.VAlign.Center;
        label.AddThemeFontOverride("font", GD.Load<Font>("res://assets/fonts/console.ttf"));
        label.AddThemeColorOverride("font_color", new Color(0.8f, 0.9f, 1.0f));
        label.AddThemeColorOverride("font_shadow_color", new Color(0.1f, 0.2f, 0.3f));
        label.AddThemeConstantOverride("shadow_as_outline", 1);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
        hud.AddChild(label);

        // Remove the message after 10 seconds
        GetTree().CreateTimer(10.0).Timeout += () => label.QueueFree();
    }
}