using Godot;

/// <summary>
/// Aldenmere Hauptplatz — verwaltet Trigger-Zonen und NPC-Präsenz.
/// </summary>
public partial class AldenmereZone : Node3D
{
    private bool _akademieTriggerActive = false;
    private bool _sudausganTriggerActive = false;

    public override void _Ready()
    {
        // Ambient-Musik läuft via AudioStreamPlayer-Node in der Szene
        GD.Print("[AldenmereZone] Hauptplatz geladen.");
    }

    // Wird von TriggerArea-Body-Entered-Signalen aufgerufen (via Scene-Verbindungen)
    public void OnAkademieTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _akademieTriggerActive = true;
        GD.Print("[AldenmereZone] Akademie-Eingang — Drücke F zum Betreten.");
    }

    public void OnAkademieTriggerBodyExited(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _akademieTriggerActive = false;
    }

    public void OnSuedausgangTriggerBodyEntered(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _sudausganTriggerActive = true;
        GD.Print("[AldenmereZone] Südausgang — Drücke F für Firewall Ruins.");
    }

    public void OnSuedausgangTriggerBodyExited(Node3D body)
    {
        if (body is not CharacterBody3D) return;
        _sudausganTriggerActive = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode == Key.F)
            {
                if (_akademieTriggerActive)
                    GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/AkademieInnen.tscn");
                else if (_sudausganTriggerActive)
                    GetTree().ChangeSceneToFile("res://scenes/world/FirewallRuins/Zone1.tscn");
            }
        }
    }
}
