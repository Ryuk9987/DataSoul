using Godot;

public partial class SentinelVisual : Node3D
{
    public override void _Ready()
    {
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null)
            ToonMaterial.Apply(body, new Color(0.55f, 0.08f, 0.08f),
                outlineColor: Colors.Black, outlineWidth: 0.03f);

        // Helm (flattened sphere — robotic visor look)
        var helm = new MeshInstance3D();
        helm.Mesh = new SphereMesh { Radius = 0.26f, Height = 0.38f };
        helm.Position = new Vector3(0, 1.12f, 0);
        helm.Scale = new Vector3(1f, 0.75f, 1f);
        ToonMaterial.Apply(helm, new Color(0.3f, 0.05f, 0.05f),
            outlineColor: Colors.Black, outlineWidth: 0.025f);
        AddChild(helm);

        // Visor glow (red eye-line)
        var visor = new MeshInstance3D();
        visor.Mesh = new BoxMesh { Size = new Vector3(0.32f, 0.06f, 0.04f) };
        visor.Position = new Vector3(0, 1.12f, 0.23f);
        var visorMat = new StandardMaterial3D();
        visorMat.AlbedoColor = new Color(1.0f, 0.1f, 0.1f);
        visorMat.EmissionEnabled = true;
        visorMat.Emission = GameColors.EnemyNormal;
        visorMat.EmissionEnergyMultiplier = 3.0f;
        visor.MaterialOverride = visorMat;
        AddChild(visor);

        // Shoulder plates
        AddShoulderPlate(new Vector3(-0.48f, 0.78f, 0), new Vector3(0, 0, 12));
        AddShoulderPlate(new Vector3(0.48f, 0.78f, 0), new Vector3(0, 0, -12));

        // Arm weapon
        var weapon = new MeshInstance3D();
        weapon.Mesh = new BoxMesh { Size = new Vector3(0.09f, 0.45f, 0.09f) };
        weapon.Position = new Vector3(0.44f, 0.3f, 0);
        weapon.RotationDegrees = new Vector3(0, 0, 8);
        var wMat = new StandardMaterial3D();
        wMat.AlbedoColor = new Color(0.25f, 0.04f, 0.04f);
        wMat.EmissionEnabled = true;
        wMat.Emission = GameColors.EnemyNormal;
        wMat.EmissionEnergyMultiplier = 1.0f;
        weapon.MaterialOverride = wMat;
        AddChild(weapon);
    }

    private void AddShoulderPlate(Vector3 pos, Vector3 rotDeg)
    {
        var plate = new MeshInstance3D();
        plate.Mesh = new BoxMesh { Size = new Vector3(0.42f, 0.12f, 0.28f) };
        plate.Position = pos;
        plate.RotationDegrees = rotDeg;
        ToonMaterial.Apply(plate, new Color(0.35f, 0.05f, 0.05f),
            outlineColor: Colors.Black, outlineWidth: 0.02f);
        AddChild(plate);
    }
}
