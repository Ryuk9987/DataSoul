using Godot;

public partial class LyraVisual : Node3D
{
    private MeshInstance3D _healAura;

    public override void _Ready()
    {
        // Hair indicator (silver sphere on top)
        var hair = new MeshInstance3D();
        var hairMesh = new SphereMesh();
        hairMesh.Radius = 0.2f;
        hairMesh.Height = 0.4f;
        hair.Mesh = hairMesh;
        hair.Position = new Vector3(0, 1.05f, 0);
        var hairMat = new StandardMaterial3D();
        hairMat.AlbedoColor = new Color(0.85f, 0.9f, 1.0f);
        hairMat.EmissionEnabled = true;
        hairMat.Emission = new Color(0.7f, 0.8f, 1.0f);
        hairMat.EmissionEnergyMultiplier = 0.5f;
        hair.MaterialOverride = hairMat;
        AddChild(hair);

        // Book (left hand)
        var book = new MeshInstance3D();
        var bookMesh = new BoxMesh();
        bookMesh.Size = new Vector3(0.3f, 0.4f, 0.05f);
        book.Mesh = bookMesh;
        book.Position = new Vector3(-0.4f, 0.55f, 0.1f);
        book.RotationDegrees = new Vector3(10, -15, 5);
        var bookMat = new StandardMaterial3D();
        bookMat.AlbedoColor = new Color(0.95f, 0.95f, 1.0f);
        book.MaterialOverride = bookMat;
        AddChild(book);

        // Healing aura (hidden by default, shown when healing)
        _healAura = new MeshInstance3D();
        var auraMesh = new SphereMesh();
        auraMesh.Radius = 1.2f;
        auraMesh.Height = 2.4f;
        _healAura.Mesh = auraMesh;
        var auraMat = new StandardMaterial3D();
        auraMat.AlbedoColor = new Color(0.2f, 1.0f, 0.4f, 0.15f);
        auraMat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        auraMat.EmissionEnabled = true;
        auraMat.Emission = GameColors.HealGreen;
        auraMat.EmissionEnergyMultiplier = 0.3f;
        auraMat.CullMode = BaseMaterial3D.CullModeEnum.Disabled;
        _healAura.MaterialOverride = auraMat;
        _healAura.Visible = false;
        AddChild(_healAura);

        // Update body material
        var body = GetParentOrNull<MeshInstance3D>();
        if (body != null)
        {
            var bodyMat = new StandardMaterial3D();
            bodyMat.AlbedoColor = GameColors.LyraColor;
            bodyMat.EmissionEnabled = true;
            bodyMat.Emission = GameColors.LyraColor * 0.2f;
            bodyMat.EmissionEnergyMultiplier = 0.2f;
            body.MaterialOverride = bodyMat;
        }
    }

    public void ShowHealEffect(float duration = 0.5f)
    {
        if (_healAura == null) return;
        _healAura.Visible = true;
        var timer = new Timer();
        timer.WaitTime = duration;
        timer.OneShot = true;
        timer.Timeout += () => { _healAura.Visible = false; timer.QueueFree(); };
        AddChild(timer);
        timer.Start();
    }
}
