using Godot;

/// <summary>
/// Attaches to the Player's MeshInstance3D and adds a head + weapon visual.
/// </summary>
public partial class PlayerVisual : Node3D
{
    public override void _Ready()
    {
        // Head (sphere on top of capsule)
        var head = new MeshInstance3D();
        var headMesh = new SphereMesh();
        headMesh.Radius = 0.22f;
        headMesh.Height = 0.44f;
        head.Mesh = headMesh;
        head.Position = new Vector3(0, 1.1f, 0);
        var headMat = new StandardMaterial3D();
        headMat.AlbedoColor = GameColors.PlayerPrimary;
        head.MaterialOverride = headMat;
        AddChild(head);

        // Weapon (cyan glowing box — right hand)
        var weapon = new MeshInstance3D();
        var weaponMesh = new BoxMesh();
        weaponMesh.Size = new Vector3(0.08f, 0.6f, 0.08f);
        weapon.Mesh = weaponMesh;
        weapon.Position = new Vector3(0.4f, 0.6f, -0.1f);
        weapon.RotationDegrees = new Vector3(0, 0, 15);
        var weaponMat = new StandardMaterial3D();
        weaponMat.AlbedoColor = GameColors.DataCyan;
        weaponMat.EmissionEnabled = true;
        weaponMat.Emission = GameColors.DataCyan;
        weaponMat.EmissionEnergyMultiplier = 1.5f;
        weapon.MaterialOverride = weaponMat;
        AddChild(weapon);

        // Update body material to player color
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null)
        {
            var bodyMat = new StandardMaterial3D();
            bodyMat.AlbedoColor = GameColors.PlayerPrimary;
            bodyMat.EmissionEnabled = true;
            bodyMat.Emission = GameColors.PlayerPrimary * 0.3f;
            bodyMat.EmissionEnergyMultiplier = 0.3f;
            body.MaterialOverride = bodyMat;
        }
    }
}
