using Godot;

public partial class EnemyAI : EnemyBase
{
    [Export] public float AggroRange = 6.0f;
    [Export] public float AttackRange = 1.5f;
    [Export] public float LoseAggroRange = 15.0f;
    [Export] public float AttackCooldown = 1.0f;
    [Export] public bool IsStationary = false; // For FirewallNode

    // Ranged settings
    [Export] public bool IsRanged = false;
    [Export] public float RangedRange = 12.0f;
    [Export] public float ProjectileCooldown = 2.0f;
    [Export] public PackedScene ProjectileScene;

    private enum AIState { Idle, Patrol, Chase, Attack, Dead }
    private AIState _state = AIState.Patrol;

    private CharacterBody3D _player;
    private NavigationAgent3D _navAgent;
    private float _attackTimer = 0f;
    private float _projectileTimer = 0f;

    // Patrol
    private Vector3[] _patrolPoints = new Vector3[3];
    private int _patrolIndex = 0;
    private float _patrolWaitTimer = 0f;

    public override void _Ready()
    {
        base._Ready();
        _navAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        // Find player
        _player = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;

        if (!IsStationary) GeneratePatrolPoints();

        Died += (e) => _state = AIState.Dead;
    }

    private void GeneratePatrolPoints()
    {
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        for (int i = 0; i < 3; i++)
        {
            _patrolPoints[i] = GlobalPosition + new Vector3(
                rng.RandfRange(-8f, 8f), 0, rng.RandfRange(-8f, 8f));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead || _state == AIState.Dead) return;

        _attackTimer -= (float)delta;
        _projectileTimer -= (float)delta;

        if (_player == null)
        {
            _player = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;
            return;
        }

        float distToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);

        switch (_state)
        {
            case AIState.Idle:
            case AIState.Patrol:
                if (!IsStationary) DoPatrol(delta);
                // Aggro check
                if (distToPlayer <= AggroRange)
                    _state = AIState.Chase;
                break;

            case AIState.Chase:
                if (distToPlayer > LoseAggroRange)
                {
                    _state = AIState.Patrol;
                    break;
                }

                if (IsRanged && distToPlayer <= RangedRange)
                {
                    // Face player for ranged
                    LookAtPlayer();
                    if (_projectileTimer <= 0)
                    {
                        ShootProjectile();
                        _projectileTimer = ProjectileCooldown;
                    }
                }
                else if (!IsRanged && distToPlayer <= AttackRange)
                {
                    _state = AIState.Attack;
                }
                else if (!IsStationary)
                {
                    MoveToward(_player.GlobalPosition, delta);
                }
                else
                {
                    LookAtPlayer();
                }
                break;

            case AIState.Attack:
                if (distToPlayer > AttackRange * 1.5f)
                {
                    _state = AIState.Chase;
                    break;
                }
                LookAtPlayer();
                if (_attackTimer <= 0)
                {
                    DoMeleeAttack();
                    _attackTimer = AttackCooldown;
                }
                break;
        }
    }

    private void DoPatrol(double delta)
    {
        if (_patrolWaitTimer > 0) { _patrolWaitTimer -= (float)delta; return; }

        var target = _patrolPoints[_patrolIndex];
        if (GlobalPosition.DistanceTo(target) < 0.5f)
        {
            _patrolIndex = (_patrolIndex + 1) % 3;
            _patrolWaitTimer = 1.5f;
            return;
        }
        MoveToward(target, delta);
    }

    private void MoveToward(Vector3 target, double delta)
    {
        if (_navAgent != null)
        {
            _navAgent.TargetPosition = target;
            var nextPos = _navAgent.GetNextPathPosition();
            var dir = (nextPos - GlobalPosition).Normalized();
            dir.Y = 0;
            Velocity = new Vector3(dir.X * Spd, Velocity.Y - 20f * (float)delta, dir.Z * Spd);
        }
        else
        {
            var dir = (target - GlobalPosition).Normalized();
            dir.Y = 0;
            Velocity = new Vector3(dir.X * Spd, Velocity.Y - 20f * (float)delta, dir.Z * Spd);
        }
        MoveAndSlide();
    }

    private void LookAtPlayer()
    {
        if (_player == null) return;
        var dir = (_player.GlobalPosition - GlobalPosition);
        dir.Y = 0;
        if (dir.LengthSquared() > 0.001f)
            LookAt(GlobalPosition + dir, Vector3.Up);
    }

    private void DoMeleeAttack()
    {
        if (_player == null) return;
        // Notify player of incoming hit (for dodge counter)
        var controller = _player as PlayerController;
        controller?.NotifyIncomingHit(0.2f);

        // Deal damage if not dodging/invincible
        if (controller != null && !controller.IsInvincible)
        {
            var playerStats = _player.GetNodeOrNull<PlayerStats>("PlayerStats");
            playerStats?.TakeDamage(Atk);
        }
    }

    private void ShootProjectile()
    {
        if (ProjectileScene == null || _player == null) return;
        var proj = ProjectileScene.Instantiate<Node3D>();
        GetParent().AddChild(proj);
        proj.GlobalPosition = GlobalPosition + Vector3.Up * 0.5f;
        // Set projectile direction toward player
        var dir = (_player.GlobalPosition - GlobalPosition).Normalized();
        if (proj.HasMethod("SetDirection"))
            proj.Call("SetDirection", dir, Atk);
    }
}
