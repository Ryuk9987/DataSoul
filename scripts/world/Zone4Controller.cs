using Godot;

/// <summary>
/// Zone 4 — Der Kern. Nach dem Boss-Tod erscheint kurz die DataWorld-Karte
/// am Monolithen (laut GDD), dann Dungeon-Abschluss.
/// Lyra kommentiert das Geschehen dramatisch.
/// </summary>
public partial class Zone4Controller : Node3D
{
    private bool _sequencePlayed = false;

    public override void _Ready()
    {
        // Warte bis Boss existiert, dann auf seinen Tod reagieren
        CallDeferred(MethodName.ConnectBoss);
    }

    private void ConnectBoss()
    {
        var boss = GetNodeOrNull<EnemyBase>("Boss");
        if (boss != null)
            boss.Died += OnBossDied;
    }

    private void OnBossDied(EnemyBase _)
    {
        if (_sequencePlayed) return;
        _sequencePlayed = true;

        // Monolith aufleuchten lassen
        var monolith = GetNodeOrNull<StaticBody3D>("MonolithCenter");
        if (monolith != null)
        {
            var mesh = monolith.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
            if (mesh != null)
            {
                var mat = new StandardMaterial3D();
                mat.AlbedoColor = new Color(0.3f, 0.0f, 0.8f);
                mat.EmissionEnabled = true;
                mat.Emission = new Color(0.5f, 0.2f, 1.0f);
                mat.EmissionEnergyMultiplier = 3.0f;
                mesh.MaterialOverride = mat;
            }
        }

        // Schritt 1: Nach 0.5s — Lyra spricht (Monolith aktiviert sich)
        var timerLyra1 = new Timer { WaitTime = 0.5f, OneShot = true };
        timerLyra1.Timeout += OnLyraMonolithComment;
        AddChild(timerLyra1);
        timerLyra1.Start();
    }

    private void OnLyraMonolithComment()
    {
        DialogueSystem.Instance?.ShowLine(
            "Lyra",
            "Der Monolith... er aktiviert sich. Das sollte nicht möglich sein.",
            3f
        );

        // Schritt 2: Nach weiteren 1s (= 0.5s + 1s = 1.5s nach Boss-Tod): Karte erscheint
        var timerMap = new Timer { WaitTime = 1.0f, OneShot = true };
        timerMap.Timeout += ShowMapMessage;
        AddChild(timerMap);
        timerMap.Start();
    }

    private void ShowMapMessage()
    {
        var hud = GetTree().Root.FindChild("PlayerHUD", true, false) as CanvasLayer;
        if (hud == null) return;

        var panel = new Panel();
        panel.CustomMinimumSize = new Vector2(600, 300);
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);

        var vbox = new VBoxContainer();
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        vbox.AddThemeConstantOverride("separation", 12);

        var title = new Label();
        title.Text = "— DATAWORLD MAP —";
        title.HorizontalAlignment = HorizontalAlignment.Center;
        title.AddThemeColorOverride("font_color", new Color(0.5f, 0.8f, 1.0f));
        title.AddThemeFontSizeOverride("font_size", 18);

        var map = new Label();
        map.Text =
            "         [ GLACIUS ❄ ]\n" +
            "              |\n" +
            "[ FERRUM ]----+----[ SYLVARA ]\n" +
            "         [ VALDRIS ★ ]\n" +
            "[ PYRAXIS ]   |   [ AQUALIS ]\n" +
            "         . NULLHEIM .";
        map.HorizontalAlignment = HorizontalAlignment.Center;
        map.AddThemeColorOverride("font_color", new Color(0.7f, 1.0f, 0.8f));
        map.AddThemeFontSizeOverride("font_size", 14);

        var sub = new Label();
        sub.Text = "The signal fades before the map completes.\n\"Find Kael. He knows—\"";
        sub.HorizontalAlignment = HorizontalAlignment.Center;
        sub.AutowrapMode = TextServer.AutowrapMode.Word;
        sub.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.6f));

        panel.AddChild(vbox);
        vbox.AddChild(title);
        vbox.AddChild(map);
        vbox.AddChild(sub);
        hud.AddChild(panel);

        // Nach 6s ausblenden → Lyra erkennt Kaels Namen
        var timer2 = new Timer { WaitTime = 6.0f, OneShot = true };
        timer2.Timeout += () =>
        {
            panel.QueueFree();
            OnLyraKaelComment();
        };
        AddChild(timer2);
        timer2.Start();
    }

    private void OnLyraKaelComment()
    {
        DialogueSystem.Instance?.ShowLine(
            "Lyra",
            "Kael. Dieser Name war auf der Liste in Zone 1. Er lebt noch.",
            5f
        );

        // Dungeon Complete nach Lyras Aussage
        var timerComplete = new Timer { WaitTime = 6.0f, OneShot = true };
        timerComplete.Timeout += ShowDungeonComplete;
        AddChild(timerComplete);
        timerComplete.Start();
    }

    private void ShowDungeonComplete()
    {
        var hud = GetTree().Root.FindChild("PlayerHUD", true, false) as CanvasLayer;
        if (hud == null) return;

        var label = new Label();
        label.Text = "— FIREWALL RUINS CLEARED —\n\nThe path to Aldenmere lies ahead.";
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AutowrapMode = TextServer.AutowrapMode.Word;
        label.CustomMinimumSize = new Vector2(500, 120);
        label.SetAnchorsPreset(Control.LayoutPreset.Center);
        label.AddThemeColorOverride("font_color", new Color(1.0f, 0.9f, 0.5f));
        label.AddThemeFontSizeOverride("font_size", 20);
        hud.AddChild(label);

        // Nach 5s verschwinden lassen
        var timer = new Timer { WaitTime = 5.0f, OneShot = true };
        timer.Timeout += () => label.QueueFree();
        AddChild(timer);
        timer.Start();
    }
}
