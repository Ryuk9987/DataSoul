using Godot;

public partial class Projectile : Area3D
{
    private Vector3 _direction = Vector3.Forward;
    private float _damage = 8f;
    private float _speed = 12f;
    private float _lifetime = 3f;
    private bool _hit = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        var timer = new Timer();
        timer.WaitTime = _lifetime;
        timer.OneShot = true;
        timer.Timeout += () => QueueFree();
        AddChild(timer);
        timer.Start();
    }

    public void SetDirection(Vector3 dir, float damage)
    {
        _direction = dir.Normalized();
        _damage = damage;
        LookAt(GlobalPosition + _direction, Vector3.Up);
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_hit) return;

        if (body is PlayerController player)
        {
            _hit = true;
            if (!player.IsInvincible)
            {
                var stats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
                stats?.TakeDamage(_damage);
                player.NotifyIncomingHit(0f); // already hit
            }
            QueueFree();
        }
        else if (body is StaticBody3D)
        {
            _hit = true;
            QueueFree();
        }
    }
}
