using Godot;

/// <summary>
/// Beschwörungsraum — Runenkreis, Partikel, Fokuspunkt-Hook für Intro-Cutscene.
/// </summary>
public partial class BeschwoerungsraumZone : Node3D
{
    [Signal] public delegate void SummoningTriggeredEventHandler();

    private bool _focusTriggerActive = false;
    private bool _ausgangTriggerActive = false;

    public override void _Ready()
    {
        GD.Print("[BeschwoerungsraumZone] Beschwörungsraum geladen.");
    }

    // Fokuspunkt-Näherung
    public void OnFocusTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _focusTriggerActive = true;
        GD.Print("[BeschwoerungsraumZone] SummoningFocus in Reichweite — Drücke F.");
    }

    public void OnFocusTriggerBodyExited(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _focusTriggerActive = false;
    }

    public void OnAusgangTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _ausgangTriggerActive = true;
        GD.Print("[BeschwoerungsraumZone] Ausgang — Drücke F zurück zur Innenhalle.");
    }

    public void OnAusgangTriggerBodyExited(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _ausgangTriggerActive = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode == Key.F)
            {
                if (_focusTriggerActive)
                {
                    GD.Print("[BeschwoerungsraumZone] SummoningTriggered! (Cutscene-Hook)");
                    EmitSignal(SignalName.SummoningTriggered);
                }
                else if (_ausgangTriggerActive)
                {
                    GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/AkademieInnen.tscn");
                }
            }
        }
    }
}
