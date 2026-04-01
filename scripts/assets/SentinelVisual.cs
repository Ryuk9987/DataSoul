using Godot;

public partial class SentinelVisual : Node3D
{
    public override void _Ready()
    {
        var redGlow = new StandardMaterial3D();
        redGlow.AlbedoColor = GameColors.EnemyNormal;
        redGlow.EmissionEnabled = true;
        redGlow.Emission = GameColors.EnemyNormal;
        redGlow.EmissionEnergyMultiplier = 0.4f;

        // Shoulder plate left
        AddShoulderPlate(new Vector3(-0.5f, 0.75f, 0), new Vector3(0, 0, 10), redGlow);
        // Shoulder plate right
        AddShoulderPlate(new Vector3(0.5f, 0.75f, 0), new Vector3(0, 0, -10), redGlow);

        // Arm weapon (right side)
        var armWeapon = new MeshInstance3D();
        var armMesh = new BoxMesh();
        armMesh.Size = new Vector3(0.1f, 0.5f, 0.1f);
        armWeapon.Mesh = armMesh;
        armWeapon.Position = new Vector3(0.45f, 0.3f, 0);
        var armMat = new StandardMaterial3D();
        armMat.AlbedoColor = new Color(0.3f, 0.05f, 0.05f);
        armMat.EmissionEnabled = true;
        armMat.Emission = GameColors.EnemyNormal * 0.5f;
        armMat.EmissionEnergyMultiplier = 0.6f;
        armWeapon.MaterialOverride = armMat;
        AddChild(armWeapon);

        // Update body material
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null) body.MaterialOverride = redGlow;
    }

    private void AddShoulderPlate(Vector3 pos, Vector3 rotDeg, StandardMaterial3D mat)
    {
        var plate = new MeshInstance3D();
        var mesh = new BoxMesh();
        mesh.Size = new Vector3(0.5f, 0.1f, 0.3f);
        plate.Mesh = mesh;
        plate.Position = pos;
        plate.RotationDegrees = rotDeg;
        plate.MaterialOverride = mat;
        AddChild(plate);
    }
}
