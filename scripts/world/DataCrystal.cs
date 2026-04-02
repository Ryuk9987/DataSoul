using Godot;

public partial class DataCrystal : StaticBody3D
{
    [Signal]
    public delegate void CrystalFoundEventHandler();

    private bool _activated = false;

    public override void _Ready()
    {
        var area = GetNodeOrNull<Area3D>("InteractArea");
        if (area != null)
            area.BodyEntered += OnPlayerEnter;
    }

    private void OnPlayerEnter(Node3D body)
    {
        if (body is PlayerController && !_activated)
        {
            _activated = true;
            ShowCrystalMessage();
            EmitSignal(nameof(CrystalFound));
        }
    }

    private void ShowCrystalMessage()
    {
        // Sequenz von 4 Zeilen — dramatische Aufzeichnung eines unbekannten Beschworenen
        var lines = new[]
        {
            new DialogueSystem.DialogueLine(
                "Aufzeichnung — Unbekannt",
                "...ich weiß nicht wie lange ich noch... der Fluch hält mich hier. Ich kann nicht zurück. Ich kann nicht kämpfen.",
                5f
            ),
            new DialogueSystem.DialogueLine(
                "Aufzeichnung — Unbekannt",
                "Wenn jemand das findet — The Root ist real. Valdris lügt. Alles was du über die Brotherhood glaubst ist eine Lüge.",
                5f
            ),
            new DialogueSystem.DialogueLine(
                "Aufzeichnung — Unbekannt",
                "Sucht Kael. Er weiß wie man hinkommt. Er weiß die Wahrheit. Er hat mir nicht geholfen — aber er konnte nicht.",
                5f
            ),
            new DialogueSystem.DialogueLine(
                "Aufzeichnung — Unbekannt",
                "Mein Name ist— [SIGNAL UNTERBROCHEN]",
                6f
            ),
        };

        DialogueSystem.Instance?.ShowSequence(lines);

        // Lyras Reaktion nach der Sequenz (Gesamtdauer der 4 Zeilen: 5+5+5+6 = 21s)
        // Loyalität erhöhen — Lyra ist erschüttert
        var loyaltyTimer = new Timer { WaitTime = 21.0f, OneShot = true };
        loyaltyTimer.Timeout += OnSequenceComplete;
        AddChild(loyaltyTimer);
        loyaltyTimer.Start();

        // Visueller Effekt: Kristall pulsiert stärker während der Aufzeichnung
        var mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (mesh?.MaterialOverride is StandardMaterial3D mat)
        {
            mat.EmissionEnergyMultiplier = 3.0f;
        }
    }

    private void OnSequenceComplete()
    {
        // Loyalität erhöhen — Lyra hört das auch und ist erschüttert
        LoyaltySystem.Instance?.AddLoyalty("lyra", 8);

        // Lyras Reaktion — nur wenn Loyalität > 40
        var loyalty = LoyaltySystem.Instance?.GetLoyalty("lyra") ?? 50;
        if (loyalty > 40)
        {
            // 2s Delay bevor Lyra spricht
            var lyraTimer = new Timer { WaitTime = 2.0f, OneShot = true };
            lyraTimer.Timeout += OnLyraSpeak;
            AddChild(lyraTimer);
            lyraTimer.Start();
        }

        // Kristall dimmt ab
        var mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (mesh?.MaterialOverride is StandardMaterial3D mat)
        {
            mat.EmissionEnergyMultiplier = 0.5f;
        }
    }

    private void OnLyraSpeak()
    {
        DialogueSystem.Instance?.ShowLine(
            "Lyra",
            "Das... das war eine echte Person. Das Königreich hat das gewusst.",
            5f
        );
    }
}
