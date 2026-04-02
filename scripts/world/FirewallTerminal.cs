using Godot;
using System.Collections.Generic;

public partial class FirewallTerminal : StaticBody3D
{
    [Export] public int TerminalId { get; set; }
    [Export] public string SecretDoorName { get; set; } = "DoorToSecret";
    [Export] public string SecretZoneName { get; set; } = "SecretRoom";

    [Signal] public delegate void TerminalActivatedEventHandler(int terminalId);

    private bool _isActivated = false;
    private bool _playerNearby = false;
    private PlayerController _nearbyPlayer = null;

    private static List<int> _activationOrder = new List<int>();
    private static int _correctOrder = 1;
    private static bool _puzzleSolved = false;
    private static bool _bypassUsed = false;

    public override void _Ready()
    {
        AddToGroup("firewall_terminals");
        var area = GetNodeOrNull<Area3D>("InteractArea");
        if (area != null)
        {
            area.BodyEntered += OnPlayerEnter;
            area.BodyExited += OnPlayerExit;
        }

        if (TerminalId == 1)
        {
            _activationOrder = new List<int>();
            _correctOrder = 1;
            _puzzleSolved = false;
            _bypassUsed = false;
        }
    }

    public override void _Process(double delta)
    {
        if (_playerNearby && _nearbyPlayer != null && Input.IsActionJustPressed("interact"))
        {
            if (_isActivated || _puzzleSolved) return;

            if (!_bypassUsed)
            {
                var stats = _nearbyPlayer.GetNodeOrNull<PlayerStats>("PlayerStats");
                if (stats != null &&
                    (stats.PlayerBackground == PlayerStats.Background.Hacker ||
                     stats.PlayerBackground == PlayerStats.Background.Programmer))
                {
                    _bypassUsed = true;
                    BypassWithExploit();
                    return;
                }
            }

            ActivateTerminal();
        }
    }

    private void OnPlayerEnter(Node3D body)
    {
        if (body is PlayerController player)
        {
            _playerNearby = true;
            _nearbyPlayer = player;
            DialogueSystem.Instance?.ShowLine("System", $"[F] Terminal aktivieren", 0f);
        }
    }

    private void OnPlayerExit(Node3D body)
    {
        if (body is PlayerController)
        {
            _playerNearby = false;
            _nearbyPlayer = null;
        }
    }

    private void BypassWithExploit()
    {
        _puzzleSolved = true;
        _isActivated = true;

        DialogueSystem.Instance?.ShowLine("System",
            "Stack-Overflow-Exploit erkannt. Terminals werden automatisch deaktiviert.", 3f);

        // Alle Terminal-Nodes visuell als aktiviert markieren
        foreach (var node in GetTree().GetNodesInGroup("firewall_terminals"))
        {
            if (node is FirewallTerminal ft)
            {
                ft._isActivated = true;
                SetTerminalColor(ft, new Color(0.0f, 1.0f, 0.3f));
            }
        }

        OpenSecretDoor();
    }

    private void ActivateTerminal()
    {
        _isActivated = true;
        _activationOrder.Add(TerminalId);
        SetTerminalColor(this, new Color(0.0f, 1.0f, 0.3f));

        bool correctSoFar = _activationOrder.Count == _correctOrder
            && _activationOrder[_correctOrder - 1] == _correctOrder;

        if (correctSoFar)
        {
            _correctOrder++;
            EmitSignal(nameof(TerminalActivated), TerminalId);

            if (_correctOrder > 3)
            {
                _puzzleSolved = true;
                OpenSecretDoor();
            }
        }
        else
        {
            // Falsche Reihenfolge
            SetTerminalColor(this, new Color(1.0f, 0.1f, 0.0f));
            DialogueSystem.Instance?.ShowLine("System",
                "Falsche Reihenfolge — Sicherheitsprotokoll aktiviert!", 2.5f);
            SpawnEnemies();
            ResetPuzzle();
        }
    }

    private void OpenSecretDoor()
    {
        var door = GetTree().Root.FindChild(SecretDoorName, true, false) as DungeonDoor;
        if (door != null)
        {
            door.Open();
            var timer = new Timer { WaitTime = 0.8f, OneShot = true };
            timer.Timeout += () => ZoneManager.Instance?.MovePlayerToZone(SecretZoneName);
            AddChild(timer);
            timer.Start();
        }
        else
        {
            GD.PrintErr($"[FirewallTerminal] DoorToSecret '{SecretDoorName}' nicht gefunden!");
        }
    }

    private void SpawnEnemies()
    {
        var wraithScene = GD.Load<PackedScene>("res://scenes/enemies/DataWraith.tscn");
        if (wraithScene == null) return;
        for (int i = 0; i < 2; i++)
        {
            var wraith = wraithScene.Instantiate<Node3D>();
            GetParent().AddChild(wraith);
            wraith.GlobalPosition = GlobalPosition + new Vector3(i * 2f, 0f, 0f);
        }
    }

    private void ResetPuzzle()
    {
        _activationOrder.Clear();
        _correctOrder = 1;
        // Terminals zurücksetzen
        foreach (var node in GetTree().GetNodesInGroup("firewall_terminals"))
        {
            if (node is FirewallTerminal ft)
            {
                ft._isActivated = false;
                SetTerminalColor(ft, new Color(0.0f, 0.5f, 1.0f)); // zurück auf blau
            }
        }
    }

    private static void SetTerminalColor(FirewallTerminal ft, Color emission)
    {
        var mesh = ft.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (mesh == null) return;
        var mat = (mesh.GetActiveMaterial(0) as StandardMaterial3D)?.Duplicate() as StandardMaterial3D;
        if (mat == null) return;
        mat.Emission = emission;
        mat.EmissionEnergyMultiplier = 1.5f;
        mesh.SetSurfaceOverrideMaterial(0, mat);
    }
}
