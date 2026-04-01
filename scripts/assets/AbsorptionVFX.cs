using Godot;

public partial class AbsorptionVFX : GpuParticles3D
{
    public override void _Ready()
    {
        Amount = 20;
        Lifetime = 1.5f;
        OneShot = true;
        Explosiveness = 0.8f;
        Emitting = false;

        var particleMat = new ParticleProcessMaterial();
        particleMat.Direction = new Vector3(0, 0.5f, 0);
        particleMat.Spread = 45f;
        particleMat.InitialVelocityMin = 3f;
        particleMat.InitialVelocityMax = 6f;
        particleMat.Gravity = new Vector3(0, -2f, 0);
        particleMat.ScaleMin = 0.05f;
        particleMat.ScaleMax = 0.12f;
        particleMat.Color = GameColors.DataCyan;
        ProcessMaterial = particleMat;

        var mesh = new SphereMesh();
        mesh.Radius = 0.05f;
        mesh.Height = 0.1f;
        DrawPass1 = mesh;

        // Auto-remove when done
        var timer = new Timer();
        timer.WaitTime = Lifetime + 0.2f;
        timer.OneShot = true;
        timer.Timeout += () => QueueFree();
        AddChild(timer);
        timer.Start();
    }

    /// <summary>
    /// Spawn absorption VFX at enemy position, flying toward player.
    /// </summary>
    public static void SpawnAt(Node parent, Vector3 from, Vector3 to)
    {
        var vfx = new AbsorptionVFX();
        parent.AddChild(vfx);
        vfx.GlobalPosition = from;

        // Aim particles toward player
        var dir = (to - from).Normalized();
        if (vfx.ProcessMaterial is ParticleProcessMaterial pm)
        {
            pm.Direction = dir;
            pm.InitialVelocityMin = (to - from).Length() * 0.5f;
            pm.InitialVelocityMax = (to - from).Length() * 0.8f;
        }

        vfx.Emitting = true;
    }
}
