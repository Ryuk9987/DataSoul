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
    private CharacterBody3D _player;

    public override void _Ready()
    {
        base._Ready();
        // Player wird von EnemyAI base._Ready() gesucht — hier nochmal als CharacterBody3D holen
        _player = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;
        SetVisibility(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_player == null)
        {
            _player = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;
            return;
        }

        float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (distance < VisibilityRange && !_isVisible)
            SetVisibility(true);

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

    public override void TakeDamage(float damage, bool isBackAttack = false)
    {
        if (_isVulnerable)
            base.TakeDamage(damage * 2f, isBackAttack);
        else
            base.TakeDamage(damage, isBackAttack);
    }

    protected override void OnAttack()
    {
        base.OnAttack();
        _isVulnerable = true;
        SetVisibility(false);
    }

    private void SetVisibility(bool visible)
    {
        _isVisible = visible;
        // MeshInstance3D-Child heißt "MeshInstance3D" in der Scene
        var mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (mesh != null)
        {
            var material = mesh.GetActiveMaterial(0) as StandardMaterial3D;
            if (material != null)
            {
                material = (StandardMaterial3D)material.Duplicate();
                material.AlbedoColor = new Color(0.2f, 0.1f, 0.3f, visible ? 0.5f : 0.0f);
                mesh.SetSurfaceOverrideMaterial(0, material);
            }
        }
        SetCollisionLayerValue(2, visible);
    }
}
