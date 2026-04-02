using Godot;

/// <summary>
/// Environmental Storytelling — Inschriften, Wandtexte, Terminals.
/// Zeigt bei Annäherung ein Interact-Label und bei F-Druck den Story-Text im DialogueSystem.
/// </summary>
public partial class EnvironmentStory : Node3D
{
    [Export] public string StoryText { get; set; } = "";
    [Export] public string InteractLabel { get; set; } = "[F] Untersuchen";

    private Label3D _label;
    private bool _playerNearby = false;
    private bool _hasBeenRead = false;

    public override void _Ready()
    {
        // Label3D holen oder erstellen
        _label = GetNodeOrNull<Label3D>("Label3D");
        if (_label == null)
        {
            _label = new Label3D();
            _label.Position = new Vector3(0, 1.8f, 0);
            _label.FontSize = 22;
            _label.Modulate = new Color(0.8f, 0.9f, 1.0f, 1.0f);
            _label.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
            AddChild(_label);
        }
        _label.Text = InteractLabel;
        _label.Visible = false;

        // InteractArea verbinden
        var area = GetNodeOrNull<Area3D>("InteractArea");
        if (area != null)
        {
            area.BodyEntered += OnBodyEntered;
            area.BodyExited  += OnBodyExited;
        }
        else
        {
            GD.PushWarning($"[EnvironmentStory] '{Name}' hat kein InteractArea-Child!");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_playerNearby) return;
        if (@event is InputEventKey key && key.Pressed && !key.Echo
            && key.PhysicalKeycode == Key.F)
        {
            ShowStory();
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is PlayerController)
        {
            _playerNearby = true;
            _label.Visible = true;
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is PlayerController)
        {
            _playerNearby = false;
            _label.Visible = false;
        }
    }

    private void ShowStory()
    {
        if (string.IsNullOrEmpty(StoryText)) return;
        DialogueSystem.Instance?.ShowLine("Inschrift", StoryText, 6f);

        // Nach dem Lesen: Label anpassen (optional visuelles Feedback)
        if (!_hasBeenRead)
        {
            _hasBeenRead = true;
            _label.Text = "[F] Erneut lesen";
        }
    }
}
