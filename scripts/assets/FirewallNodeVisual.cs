using Godot;

public partial class FirewallNodeVisual : Node3D
{
    private MeshInstance3D _core;
    private float _pulseTimer = 0f;

    public override void _Ready()
    {
        // Antenna
        var antenna = new MeshInstance3D();
        var antMesh = new CylinderMesh();
        antMesh.TopRadius = 0.04f;
        antMesh.BottomRadius = 0.04f;
        antMesh.Height = 1.0f;
        antenna.Mesh = antMesh;
        antenna.Position = new Vector3(0, 1.75f, 0);
        var antMat = new StandardMaterial3D();
        antMat.AlbedoColor = new Color(0.3f, 0.3f, 0.4f);
        antenna.MaterialOverride = antMat;
        AddChild(antenna);

        // Energy core (pulsing cyan sphere)
        _core = new MeshInstance3D();
        var coreMesh = new SphereMesh();
        coreMesh.Radius = 0.2f;
        coreMesh.Height = 0.4f;
        _core.Mesh = coreMesh;
        _core.Position = new Vector3(0, 1.0f, 0);
        var coreMat = new StandardMaterial3D();
        coreMat.AlbedoColor = GameColors.DataCyan;
        coreMat.EmissionEnabled = true;
        coreMat.Emission = GameColors.DataCyan;
        coreMat.EmissionEnergyMultiplier = 2.0f;
        _core.MaterialOverride = coreMat;
        AddChild(_core);

        // Ring around core
        var ring = new MeshInstance3D();
        var ringMesh = new TorusMesh();
        ringMesh.InnerRadius = 0.4f;
        ringMesh.OuterRadius = 0.55f;
        ring.Mesh = ringMesh;
        ring.Position = new Vector3(0, 1.0f, 0);
        var ringMat = new StandardMaterial3D();
        ringMat.AlbedoColor = GameColors.DataCyan * 0.5f;
        ringMat.EmissionEnabled = true;
        ringMat.Emission = GameColors.DataCyan;
        ringMat.EmissionEnergyMultiplier = 0.8f;
        ring.MaterialOverride = ringMat;
        AddChild(ring);
    }

    public override void _Process(double delta)
    {
        // Pulse the core emission
        _pulseTimer += (float)delta * 2f;
        if (_core?.MaterialOverride is StandardMaterial3D mat)
            mat.EmissionEnergyMultiplier = 1.5f + Mathf.Sin(_pulseTimer) * 0.8f;
    }
}
