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

    public override void TakeDamage(int damage)
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
            else if (damage >= 15) // Heavy attack threshold
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
            base.TakeDamage(damage);
        }
    }

    protected override void UpdatePhase()
    {
        if (Health < MaxHealth * 0.5f && !_isShieldActive)
        {
            _isShieldActive = true;
            // Visual feedback for shield activation
            var material = (StandardMaterial3D)GetNode<MeshInstance3D>("Mesh").MaterialOverlay;
            material.EmissionEnabled = true;
            material.Emission = new Color(0.2f, 0.5f, 1.0f);
        }
    }

    private void FireProjectiles()
    {
        for (int i = 0; i < ProjectileCount; i++)
        {
            var projectile = GD.Load<PackedScene>("res://scenes/projectiles/FirewallProjectile.tscn").Instantiate<Area3D>();
            GetParent().AddChild(projectile);
            projectile.GlobalPosition = GlobalPosition;

            // Calculate angle for fan pattern
            float angle = (i - (ProjectileCount - 1) / 2.0f) * 30.0f;
            Vector3 direction = (GlobalRotation.Y + Mathf.DegToRad(angle)).Normalized();

            projectile.LinearVelocity = direction * 10.0f;
        }
    }
}