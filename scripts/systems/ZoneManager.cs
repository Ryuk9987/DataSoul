using Godot;

public partial class ZoneManager : Node
{
    public static ZoneManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public void MovePlayerToZone(string zoneName)
    {
        var player = GetTree().Root.FindChild("Player", true, false) as Node3D;
        if (player == null)
        {
            GD.PrintErr("[ZoneManager] Player nicht gefunden!");
            return;
        }

        // Alle direkten Children des Parents (FirewallRuins) durchsuchen
        Node3D zone = null;
        var parent = GetParent();
        if (parent != null)
        {
            foreach (Node child in parent.GetChildren())
            {
                if (child.Name == zoneName && child is Node3D n)
                {
                    zone = n;
                    break;
                }
            }
        }

        if (zone == null)
        {
            GD.PrintErr($"[ZoneManager] Zone '{zoneName}' nicht gefunden! Parent-Children: ");
            if (parent != null)
                foreach (Node c in parent.GetChildren())
                    GD.Print($"  - {c.Name} ({c.GetType().Name})");
            return;
        }

        GD.Print($"[ZoneManager] Teleport → {zoneName} @ {zone.GlobalPosition}");
        player.GlobalPosition = zone.GlobalPosition + new Vector3(0, 1.5f, 2f);

        if (zoneName == "SecretRoom")
        {
            var lyraDialogue = GetTree().Root.FindChild("LyraDialogue", true, false) as LyraDialogue;
            lyraDialogue?.OnSecretRoomFound();
        }
    }

    public void TransitionTo(string scenePath)
    {
        GetTree().ChangeSceneToFile(scenePath);
    }
}
