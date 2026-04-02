using Godot;
using System.Collections.Generic;

public partial class DungeonZone : Node3D
{
    [Signal]
    public delegate void ZoneClearedEventHandler();

    private List<EnemyBase> _enemies = new List<EnemyBase>();
    private List<DungeonDoor> _doors = new List<DungeonDoor>();
    private bool _cleared = false;
    private int _totalEnemies = 0;
    private int _deadCount = 0;

    public override void _Ready()
    {
        // CallDeferred: alle Child-Export-Properties sind erst nach _Ready des Parents gesetzt
        CallDeferred(MethodName.InitZone);
    }

    private void InitZone()
    {
        foreach (Node child in GetChildren())
        {
            if (child is EnemyBase enemy)
            {
                _enemies.Add(enemy);
                enemy.Died += (_) =>
                {
                    _deadCount++;
                    GD.Print($"[DungeonZone:{Name}] Enemy died. {_deadCount}/{_totalEnemies}");
                    CheckCleared();
                };
            }
            else if (child is DungeonDoor door)
            {
                _doors.Add(door);
            }
        }

        _totalEnemies = _enemies.Count;
        GD.Print($"[DungeonZone:{Name}] Registered {_totalEnemies} enemies, {_doors.Count} doors.");

        // Nur automatisch öffnen wenn keine Enemies UND keine Tür mit TargetZone
        // (verhindert dass SecretRoom-ExitDoor beim Start sofort teleportiert)
        if (_totalEnemies == 0)
        {
            bool hasAutoTeleportDoor = false;
            foreach (var door in _doors)
                if (!door.IsLocked && !string.IsNullOrEmpty(door.TargetZoneName)) { hasAutoTeleportDoor = true; break; }

            if (!hasAutoTeleportDoor)
                UnlockDoors();
        }
    }

    private void CheckCleared()
    {
        if (_cleared) return;
        if (_deadCount >= _totalEnemies && _totalEnemies > 0)
        {
            _cleared = true;
            GD.Print($"[DungeonZone:{Name}] CLEARED!");
            EmitSignal(nameof(ZoneCleared));
            UnlockDoors();
        }
    }

    private void UnlockDoors()
    {
        foreach (var door in _doors)
        {
            // Explizit gesperrte Türen (z.B. DoorToSecret) werden von DungeonZone nicht geöffnet —
            // sie haben ihr eigenes Öffnungs-System (Terminal-Puzzle, etc.)
            if (door.IsLocked && !string.IsNullOrEmpty(door.TargetZoneName)
                && door.Name.ToString().Contains("Secret"))
            {
                GD.Print($"[DungeonZone:{Name}] Skipping locked special door: {door.Name}");
                continue;
            }

            GD.Print($"[DungeonZone:{Name}] Door={door.Name} TargetZone='{door.TargetZoneName}'");
            door.Open();

            if (!string.IsNullOrEmpty(door.TargetZoneName))
            {
                var targetName = door.TargetZoneName;
                GD.Print($"[DungeonZone:{Name}] Teleporting to {targetName} in 0.8s...");
                var timer = new Timer { WaitTime = 0.8f, OneShot = true };
                timer.Timeout += () =>
                {
                    var zm = ZoneManager.Instance
                        ?? GetTree().Root.FindChild("ZoneManager", true, false) as ZoneManager;
                    if (zm != null)
                        zm.MovePlayerToZone(targetName);
                    else
                        GD.PrintErr($"[DungeonZone] ZoneManager nicht gefunden!");
                };
                AddChild(timer);
                timer.Start();
                break;
            }
        }
    }
}
