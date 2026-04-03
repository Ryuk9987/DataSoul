using Godot;

/// <summary>
/// Akademie Innenhalle — Trigger für Keller (Beschwörungsraum) und Ausgang.
/// </summary>
public partial class AkademieInnenZone : Node3D
{
    private bool _kellerTriggerActive = false;
    private bool _ausgangTriggerActive = false;

    public override void _Ready()
    {
        GD.Print("[AkademieInnenZone] Innenhalle geladen.");
    }

    public void OnKellerTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _kellerTriggerActive = true;
        GD.Print("[AkademieInnenZone] Treppenabgang — Drücke F für Beschwörungsraum.");
    }

    public void OnKellerTriggerBodyExited(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _kellerTriggerActive = false;
    }

    public void OnAusgangTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _ausgangTriggerActive = true;
        GD.Print("[AkademieInnenZone] Ausgang — Drücke F zurück zum Hauptplatz.");
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
                if (_kellerTriggerActive)
                    GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/Beschwoerungsraum.tscn");
                else if (_ausgangTriggerActive)
                    GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/Aldenmere.tscn");
            }
        }
    }
}
