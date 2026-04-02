using Godot;

public partial class OverseerUnit7 : BossBase
{
    [Export]
    public float ProjectileCooldown { get; set; } = 4.0f;
    [Export]
    public int ProjectileCount { get; set; } = 3;
    [Export]
    public float ShieldHealth { get; set; } = 150.0f;
    [Export]
    public int ShieldBreakThreshold { get; set; } = 3;

    private float _projectileTimer = 0.0f;
    private bool _isShieldActive = false;
    private float _shieldHealth;
    private int _shieldHits = 0;

    public override void _Ready()
    {
        base._Ready();
        _shieldHealth = ShieldHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_projectileTimer > 0)
        {
            _projectileTimer -= (float)delta;
        }
        else if (!_isShieldActive)
        {
            FireProjectiles();
            _projectileTimer = ProjectileCooldown;
        }
    }

    public override void TakeDamage(float damage, bool isBackAttack = false)
    {
        if (_isShieldActive)
        {
            _shieldHealth -= damage;
            if (_shieldHealth <= 0)
            {
                _isShieldActive = false;
                _shieldHealth = ShieldHealth;
                _shieldHits = 0;
            }
            else if (damage >= 15f) // Heavy attack threshold
            {
                _shieldHits++;
                if (_shieldHits >= ShieldBreakThreshold)
                {
                    _isShieldActive = false;
                    _shieldHealth = ShieldHealth;
                    _shieldHits = 0;
                }
            }
        }
        else
        {
            base.TakeDamage(damage, isBackAttack);
        }
    }

    protected override void OnPhaseChange(int newPhase)
    {
        if (newPhase >= 2 && !_isShieldActive)
        {
            _isShieldActive = true;
            // Visual feedback for shield activation
            var meshInst = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
            if (meshInst != null)
            {
                var material = (meshInst.GetActiveMaterial(0) as StandardMaterial3D)?.Duplicate() as StandardMaterial3D;
                if (material != null)
                {
                    material.EmissionEnabled = true;
                    material.Emission = new Color(0.2f, 0.5f, 1.0f);
                    meshInst.SetSurfaceOverrideMaterial(0, material);
                }
            }
        }
    }

    private void FireProjectiles()
    {
        var projectileScene = GD.Load<PackedScene>("res://scenes/projectiles/FirewallProjectile.tscn");
        if (projectileScene == null) return;

        for (int i = 0; i < ProjectileCount; i++)
        {
            var projectile = projectileScene.Instantiate<Node3D>();
            GetParent().AddChild(projectile);
            projectile.GlobalPosition = GlobalPosition;

            // Build a direction vector from the boss's Y rotation + fan offset angle
            float angleOffset = (i - (ProjectileCount - 1) / 2.0f) * 30.0f;
            float totalAngle = GlobalRotation.Y + Mathf.DegToRad(angleOffset);
            Vector3 direction = new Vector3(Mathf.Sin(totalAngle), 0f, Mathf.Cos(totalAngle)).Normalized();

            // Use the Projectile API (SetDirection) instead of LinearVelocity
            if (projectile.HasMethod("SetDirection"))
                projectile.Call("SetDirection", direction, Atk);
        }
    }
}
