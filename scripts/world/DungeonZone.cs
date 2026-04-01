using Godot;
using System.Collections.Generic;

public partial class DungeonZone : Node
{
    [Signal]
    public delegate void ZoneClearedEventHandler();

    private List<Node> _enemies = new List<Node>();
    private List<DungeonDoor> _doors = new List<DungeonDoor>();

    public override void _Ready()
    {
        // Find all enemies and doors in the zone
        foreach (Node child in GetChildren())
        {
            if (child is EnemyAI enemy)
            {
                _enemies.Add(child);
                enemy.TreeExited += () => OnEnemyDied(enemy);
            }
            else if (child is DungeonDoor door)
            {
                _doors.Add(door);
            }
        }
    }

    public void OnZoneEnter()
    {
        // Zone entered logic
    }

    public void OnZoneExit()
    {
        // Zone exited logic
    }

    private void OnEnemyDied(EnemyAI enemy)
    {
        _enemies.Remove(enemy);
        CheckCleared();
    }

    private void CheckCleared()
    {
        if (_enemies.Count == 0)
        {
            EmitSignal(nameof(ZoneCleared));
            foreach (var door in _doors)
            {
                door.Open();
            }
        }
    }
}