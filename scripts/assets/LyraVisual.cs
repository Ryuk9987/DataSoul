using Godot;

public partial class LyraVisual : Node3D
{
    private MeshInstance3D _healAura;

    public override void _Ready()
    {
        // Body — light blue toon
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null)
            ToonMaterial.Apply(body, GameColors.LyraColor,
                outlineColor: new Color(0.1f, 0.2f, 0.4f), outlineWidth: 0.028f);

        // Face / skin
        var head = new MeshInstance3D();
        head.Mesh = new SphereMesh { Radius = 0.21f, Height = 0.42f };
        head.Position = new Vector3(0, 1.1f, 0);
        ToonMaterial.Apply(head, new Color(0.9f, 0.78f, 0.72f),
            outlineColor: new Color(0.1f, 0.2f, 0.4f), outlineWidth: 0.022f);
        AddChild(head);

        // Silver-grey hair (akademische Färbung laut GDD)
        var hair = new MeshInstance3D();
        hair.Mesh = new SphereMesh { Radius = 0.25f, Height = 0.32f };
        hair.Position = new Vector3(0, 1.3f, 0.02f);
        hair.Scale = new Vector3(1f, 0.55f, 1f);
        ToonMaterial.Apply(hair, new Color(0.78f, 0.82f, 0.9f),
            outlineColor: Colors.Black, outlineWidth: 0.02f);
        AddChild(hair);

        // Akademie-Robe (dunkelblau mit Gold-Akzent laut GDD)
        var robe = new MeshInstance3D();
        robe.Mesh = new CylinderMesh { TopRadius = 0.22f, BottomRadius = 0.28f, Height = 0.35f };
        robe.Position = new Vector3(0, 0.55f, 0);
        ToonMaterial.Apply(robe, new Color(0.12f, 0.18f, 0.45f),
            outlineColor: new Color(0.05f, 0.08f, 0.25f), outlineWidth: 0.025f);
        AddChild(robe);

        // Gold-Stickerei (Collar)
        var collar = new MeshInstance3D();
        collar.Mesh = new CylinderMesh { TopRadius = 0.21f, BottomRadius = 0.21f, Height = 0.07f };
        collar.Position = new Vector3(0, 0.75f, 0);
        var collarMat = new StandardMaterial3D();
        collarMat.AlbedoColor = new Color(0.9f, 0.75f, 0.2f);
        collarMat.EmissionEnabled = true;
        collarMat.Emission = new Color(0.8f, 0.6f, 0.1f);
        collarMat.EmissionEnergyMultiplier = 0.4f;
        collar.MaterialOverride = collarMat;
        AddChild(collar);

        // Lehrbuch (linke Hand)
        var book = new MeshInstance3D();
        book.Mesh = new BoxMesh { Size = new Vector3(0.28f, 0.36f, 0.06f) };
        book.Position = new Vector3(-0.42f, 0.55f, 0.08f);
        book.RotationDegrees = new Vector3(10, -12, 8);
        ToonMaterial.Apply(book, new Color(0.92f, 0.92f, 0.96f),
            outlineColor: new Color(0.3f, 0.3f, 0.5f), outlineWidth: 0.015f);
        // Buchseiten-Indikator
        var pages = new MeshInstance3D();
        pages.Mesh = new BoxMesh { Size = new Vector3(0.22f, 0.3f, 0.01f) };
        pages.Position = new Vector3(0, 0, 0.04f);
        var pagesMat = new StandardMaterial3D { AlbedoColor = Colors.White };
        pages.MaterialOverride = pagesMat;
        book.AddChild(pages);
        AddChild(book);

        // Healing aura (versteckt, erscheint beim Heilen)
        _healAura = new MeshInstance3D();
        _healAura.Mesh = new SphereMesh { Radius = 1.1f, Height = 2.2f };
        var auraMat = new StandardMaterial3D();
        auraMat.AlbedoColor = new Color(0.2f, 1.0f, 0.4f, 0.12f);
        auraMat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        auraMat.EmissionEnabled = true;
        auraMat.Emission = GameColors.HealGreen;
        auraMat.EmissionEnergyMultiplier = 0.4f;
        auraMat.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
        _healAura.MaterialOverride = auraMat;
        _healAura.Visible = false;
        AddChild(_healAura);
    }

    public void ShowHealEffect(float duration = 0.6f)
    {
        if (_healAura == null) return;
        _healAura.Visible = true;
        var timer = new Timer { WaitTime = duration, OneShot = true };
        timer.Timeout += () => { if (IsInstanceValid(_healAura)) _healAura.Visible = false; timer.QueueFree(); };
        AddChild(timer);
        timer.Start();
    }
}
