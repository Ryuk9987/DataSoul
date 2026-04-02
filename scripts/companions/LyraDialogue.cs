using Godot;

/// <summary>
/// Lyra's situational dialogue triggers — laut GDD: überpräzise, sozial unbeholfen,
/// zitiert ihr Lehrbuch, kompensiert Unsicherheit mit Fachwissen.
/// </summary>
public partial class LyraDialogue : Node
{
    private const string LYRA = "Lyra";

    private PlayerStats _playerStats;
    private bool _lowHpTriggered = false;
    private bool _secretRoomFound = false;
    private bool _bossDefeated = false;
    private bool _loyalty30Triggered = false;
    private bool _loyalty50Triggered = false;
    private bool _loyalty75Triggered = false;
    private float _combatCooldown = 0f;
    private float _killCooldown = 0f;

    private static readonly string[] _combatStartLines =
    {
        "Kapitel 7 der Kampfdoktrin: Ziele priorisieren. Du machst das... ausreichend.",
        "Kampf initiiert. Ich halte dich am Leben. Bitte kooperiere.",
        "Laut Protokoll sollte ich warnen: Das hier ist gefährlich. Betrachte dich als gewarnt."
    };

    private static readonly string[] _enemyKilledLines =
    {
        "Datensignatur erfolgreich absorbiert. Faszinierend.",
        "Gut gekämpft. Relativ gesehen.",
        "Ziel eliminiert. Lehrbuch-konform.",
        "Die Absorption... das ist nicht in keinem Akademie-Skript beschrieben."
    };

    private static readonly string[] _lowHpLines =
    {
        "Deine Vitalwerte sind kritisch! Seite 42 empfiehlt dringend Rückzug!",
        "Ich heile — halt durch! Bitte. Ich meine... das ist ein Befehl.",
        "HP unter 30%. Das ist statistisch... sehr schlecht."
    };

    public override void _Ready()
    {
        CallDeferred(MethodName.ConnectSignals);
    }

    public override void _Process(double delta)
    {
        if (_combatCooldown > 0) _combatCooldown -= (float)delta;
        if (_killCooldown > 0) _killCooldown -= (float)delta;
        CheckLoyaltyMilestones();
    }

    private void CheckLoyaltyMilestones()
    {
        var loyalty = LoyaltySystem.Instance?.GetLoyalty("lyra") ?? 0;

        if (!_loyalty30Triggered && loyalty >= 30)
        {
            _loyalty30Triggered = true;
            DialogueSystem.Instance?.ShowLine(LYRA,
                "Du kämpfst anders als alle Helden die ich in den Büchern gelesen habe. Weniger... heldenhaft. Aber irgendwie funktioniert es.", 6f);
        }
        else if (!_loyalty50Triggered && loyalty >= 50)
        {
            _loyalty50Triggered = true;
            DialogueSystem.Instance?.ShowSequence(new[]
            {
                new DialogueSystem.DialogueLine(LYRA, "Hast du das Brotherhood-Emblem an den Terminals gesehen?", 4f),
                new DialogueSystem.DialogueLine(LYRA, "Ich kenne dieses Symbol. Aus Archiven die... eigentlich gesperrt sein sollten.", 5f),
            });
        }
        else if (!_loyalty75Triggered && loyalty >= 75)
        {
            _loyalty75Triggered = true;
            DialogueSystem.Instance?.ShowSequence(new[]
            {
                new DialogueSystem.DialogueLine(LYRA, "Ich glaube nicht mehr alles was sie mir beigebracht haben.", 4f),
                new DialogueSystem.DialogueLine(LYRA, "Das ist... unangenehm zu sagen. Aber ich glaube, du solltest es wissen.", 5f),
            });
        }
    }

    private void ConnectSignals()
    {
        // Player HP
        var player = GetTree().Root.FindChild("Player", true, false) as Node3D;
        if (player != null)
        {
            _playerStats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
            if (_playerStats != null)
                _playerStats.HealthChanged += OnPlayerHealthChanged;
        }

        // Enemy deaths
        GetTree().Connect("node_added", new Callable(this, MethodName.OnNodeAdded));

        // Zone cleared — DungeonZone emittiert ZoneCleared
        // Alle DungeonZone-Nodes suchen (nicht via Gruppe, sondern FindChildren)
        var allNodes = GetTree().Root.FindChildren("Zone*", "Node", true, false);
        foreach (Node zone in allNodes)
        {
            if (zone.HasSignal("ZoneCleared"))
                zone.Connect("ZoneCleared", new Callable(this, MethodName.OnZoneCleared));
        }

        // Intro mit kurzem Delay damit DialogueSystem.Instance sicher bereit ist
        var timer = new Timer { WaitTime = 1.5f, OneShot = true };
        timer.Timeout += PlayIntroLine;
        AddChild(timer);
        timer.Start();
    }

    private void PlayIntroLine()
    {
        if (DialogueSystem.Instance == null)
        {
            GD.PrintErr("[LyraDialogue] DialogueSystem not found!");
            return;
        }
        DialogueSystem.Instance.ShowLine(LYRA,
            "Laut Protokoll sollte ich fragen: Bist du verletzt? Du bist gerade durch einen Datenstrom gefallen, also... ja, vermutlich.");
    }

    private void OnNodeAdded(Node node)
    {
        if (node is EnemyBase enemy)
            enemy.Died += OnEnemyDied;
    }

    private void OnEnemyDied(EnemyBase enemy)
    {
        if (_killCooldown > 0) return;
        _killCooldown = 8f;

        if (enemy is BossBase && !_bossDefeated)
        {
            _bossDefeated = true;
            LoyaltySystem.Instance?.AddLoyalty("lyra", 10);
            DialogueSystem.Instance?.ShowSequence(new[]
            {
                new DialogueSystem.DialogueLine(LYRA, "Das war kein normaler Prozess.", 4f),
                new DialogueSystem.DialogueLine(LYRA, "Diese Datensignatur... sie ist uralt. Viel älter als die Ruinen.", 5f),
                new DialogueSystem.DialogueLine(LYRA, "Ich glaube, das Königreich weiß davon. Und hat es... versteckt.", 5f)
            });
        }
        else
        {
            var lines = _enemyKilledLines;
            DialogueSystem.Instance?.ShowLine(LYRA, lines[GD.RandRange(0, lines.Length - 1)], 3f);
        }
    }

    private void OnPlayerHealthChanged(float current, float max)
    {
        float pct = max > 0 ? current / max : 1f;

        if (pct < 0.3f && !_lowHpTriggered)
        {
            _lowHpTriggered = true;
            var lines = _lowHpLines;
            DialogueSystem.Instance?.ShowLine(LYRA, lines[GD.RandRange(0, lines.Length - 1)], 4f);
        }
        else if (pct > 0.5f)
        {
            _lowHpTriggered = false; // Reset so it can trigger again
        }
    }

    public void OnCombatStart()
    {
        if (_combatCooldown > 0) return;
        _combatCooldown = 30f;
        var lines = _combatStartLines;
        DialogueSystem.Instance?.ShowLine(LYRA, lines[GD.RandRange(0, lines.Length - 1)], 3.5f);
    }

    private void OnZoneCleared()
    {
        LoyaltySystem.Instance?.AddLoyalty("lyra", 3);
        DialogueSystem.Instance?.ShowLine(LYRA,
            GD.RandRange(0, 1) == 0
                ? "Zone gesäubert. Wir können weiter."
                : "Das... war eigentlich beeindruckend. Ich meine — effizient. Statistisch.", 3.5f);
    }

    public void OnSecretRoomFound()
    {
        if (_secretRoomFound) return;
        _secretRoomFound = true;
        LoyaltySystem.Instance?.AddLoyalty("lyra", 5);
        DialogueSystem.Instance?.ShowSequence(new[]
        {
            new DialogueSystem.DialogueLine(LYRA, "Das ist nicht in meinen Karten.", 3f),
            new DialogueSystem.DialogueLine(LYRA, "Das... sollte nicht existieren.", 4f)
        });
    }
}
