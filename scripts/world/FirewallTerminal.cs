using Godot;
using System.Collections.Generic;

public partial class FirewallTerminal : Area3D
{
    [Export]
    public int TerminalId { get; set; }

    [Signal]
    public delegate void TerminalActivatedEventHandler(int terminalId);

    private bool _isActivated = false;
    private static List<int> _activationOrder = new List<int>();
    private static int _correctOrder = 1;

    public override void _Ready()
    {
        BodyEntered += OnPlayerEnter;
    }

    private void OnPlayerEnter(Node body)
    {
        if (body is PlayerController player && !_isActivated)
        {
            ActivateTerminal();
        }
    }

    private void ActivateTerminal()
    {
        _isActivated = true;
        _activationOrder.Add(TerminalId);

        // Check if the order is correct
        if (_activationOrder.Count == _correctOrder && _activationOrder[_correctOrder - 1] == _correctOrder)
        {
            _correctOrder++;
            EmitSignal(nameof(TerminalActivated), TerminalId);

            if (_correctOrder > 3)
            {
                // All terminals activated in correct order
                GetTree().CallGroup("Doors", "Open");
            }
        }
        else
        {
            // Wrong order - spawn enemies
            SpawnEnemies();
            ResetActivationOrder();
        }
    }

    private void SpawnEnemies()
    {
        // Spawn 2 Data Wraiths
        for (int i = 0; i < 2; i++)
        {
            var wraith = GD.Load<PackedScene>("res://scenes/enemies/DataWraith.tscn").Instantiate();
            GetParent().AddChild(wraith);
            wraith.GlobalPosition = GlobalPosition + new Vector3(i * 2, 0, 0);
        }
    }

    private void ResetActivationOrder()
    {
        _activationOrder.Clear();
        _correctOrder = 1;
    }
}