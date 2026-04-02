using Godot;

public partial class PlayerVisual : Node3D
{
    public override void _Ready()
    {
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null)
            ToonMaterial.Apply(body, GameColors.PlayerPrimary,
                outlineColor: new Color(0.1f, 0.0f, 0.2f), outlineWidth: 0.03f);

        // Head — slightly brighter tone
        var head = new MeshInstance3D();
        var headMesh = new SphereMesh { Radius = 0.22f, Height = 0.44f };
        head.Mesh = headMesh;
        head.Position = new Vector3(0, 1.1f, 0);
        ToonMaterial.Apply(head, new Color(0.85f, 0.72f, 0.65f),  // skin tone
            outlineColor: new Color(0.1f, 0.0f, 0.2f), outlineWidth: 0.025f);
        AddChild(head);

        // Hair — dark with slight blue tint (protagonist energy)
        var hair = new MeshInstance3D();
        var hairMesh = new SphereMesh { Radius = 0.24f, Height = 0.3f };
        hair.Mesh = hairMesh;
        hair.Position = new Vector3(0, 1.28f, 0.03f);
        hair.Scale = new Vector3(1f, 0.6f, 1f);
        ToonMaterial.Apply(hair, new Color(0.12f, 0.08f, 0.25f),
            outlineColor: Colors.Black, outlineWidth: 0.02f);
        AddChild(hair);

        // Scarf / collar detail (cyan — digital world motif)
        var scarf = new MeshInstance3D();
        var scarfMesh = new CylinderMesh { TopRadius = 0.22f, BottomRadius = 0.22f, Height = 0.12f };
        scarf.Mesh = scarfMesh;
        scarf.Position = new Vector3(0, 0.78f, 0);
        ToonMaterial.Apply(scarf, GameColors.DataCyan,
            outlineColor: new Color(0.0f, 0.3f, 0.5f), outlineWidth: 0.015f);
        AddChild(scarf);

        // Weapon — glowing data-sword (cyan blade, dark hilt)
        var hilt = new MeshInstance3D();
        var hiltMesh = new BoxMesh { Size = new Vector3(0.1f, 0.18f, 0.1f) };
        hilt.Mesh = hiltMesh;
        hilt.Position = new Vector3(0.42f, 0.55f, -0.05f);
        hilt.RotationDegrees = new Vector3(0, 0, 12);
        ToonMaterial.Apply(hilt, new Color(0.2f, 0.15f, 0.3f));
        AddChild(hilt);

        var blade = new MeshInstance3D();
        var bladeMesh = new BoxMesh { Size = new Vector3(0.05f, 0.65f, 0.03f) };
        blade.Mesh = bladeMesh;
        blade.Position = new Vector3(0.42f, 0.96f, -0.05f);
        blade.RotationDegrees = new Vector3(0, 0, 12);
        var bladeMat = new ShaderMaterial();
        // Blade bleibt StandardMaterial mit Emission für den Glow-Effekt
        var bladeStd = new StandardMaterial3D();
        bladeStd.AlbedoColor = GameColors.DataCyan;
        bladeStd.EmissionEnabled = true;
        bladeStd.Emission = GameColors.DataCyan;
        bladeStd.EmissionEnergyMultiplier = 2.0f;
        blade.MaterialOverride = bladeStd;
        AddChild(blade);
    }
}
