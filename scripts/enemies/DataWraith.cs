using Godot;

public partial class DataWraith : EnemyAI
{
    [Export]
    public float VisibilityRange { get; set; } = 3.0f;
    [Export]
    public float InvisibilityCooldown { get; set; } = 2.0f;
    [Export]
    public float VulnerabilityWindow { get; set; } = 0.5f;

    private bool _isVisible = false;
    private float _invisibilityTimer = 0.0f;
    private float _vulnerabilityTimer = 0.0f;
    private bool _isVulnerable = false;
    private PlayerController _player;

    public override void _Ready()
    {
        base._Ready();
        _player = GetNode<PlayerController>("/root/Player");
        SetVisibility(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_player == null) return;

        float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (distance < VisibilityRange && !_isVisible)
        {
            SetVisibility(true);
        }

        if (_isVisible)
        {
            _invisibilityTimer += (float)delta;
            if (_invisibilityTimer >= InvisibilityCooldown)
            {
                SetVisibility(false);
                _invisibilityTimer = 0.0f;
            }
        }

        if (_isVulnerable)
        {
            _vulnerabilityTimer += (float)delta;
            if (_vulnerabilityTimer >= VulnerabilityWindow)
            {
                _isVulnerable = false;
                _vulnerabilityTimer = 0.0f;
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        if (_isVulnerable)
        {
            base.TakeDamage(damage * 2); // Double damage during vulnerability window
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        _isVulnerable = true;
        SetVisibility(false);
    }

    private void SetVisibility(bool visible)
    {
        _isVisible = visible;
        var material = (StandardMaterial3D)GetNode<MeshInstance3D>("Mesh").MaterialOverlay;
        material.AlbedoColor = new Color(0.2f, 0.1f, 0.3f, visible ? 1.0f : 0.0f);
        SetCollisionLayerBit(1, visible); // Toggle collision layer
    }
}