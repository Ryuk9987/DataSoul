using Godot;

public partial class PlayerController : CharacterBody3D
{
    [Signal] public delegate void DodgeCounterEventHandler();
    [Signal] public delegate void DodgeStartedEventHandler();

    private PlayerStats _stats;
    private PlayerCamera _camera;
    private DataGauge _dataGauge;
    private PlayerCombat _combat;
    private AnimationPlayer _animPlayer;
    private Node3D _characterMesh;

    private const float DODGE_DURATION = 0.3f;
    private const float DODGE_SPEED = 12.0f;
    private const float GRAVITY = -20.0f;

    private bool _isDodging = false;
    private bool _isInvincible = false;
    private float _dodgeTimer = 0f;
    private Vector3 _dodgeDirection = Vector3.Zero;

    // Dodge counter window: brief window where dodge counts as counter
    private bool _hitIncoming = false;
    private float _hitIncomingTimer = 0f;

    public bool IsInvincible => _isInvincible;
    public bool IsBlocking { get; private set; } = false;

    public override void _Ready()
    {
        _stats = GetNodeOrNull<PlayerStats>("PlayerStats");
        _camera = GetNodeOrNull<PlayerCamera>("PlayerCamera");
        _dataGauge = GetNodeOrNull<DataGauge>("DataGauge");
        _combat = GetNodeOrNull<PlayerCombat>("PlayerCombat");
        // CharacterMesh + AnimationPlayer aus KayKit Knight GLB
        _characterMesh = GetNodeOrNull<Node3D>("CharacterMesh");
        _animPlayer = GetNodeOrNull<AnimationPlayer>("CharacterMesh/kaykit_knight/AnimationPlayer");
        AddToGroup("player");
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // Gravity
        if (!IsOnFloor())
            velocity.Y += GRAVITY * (float)delta;

        // Dodge timer
        if (_isDodging)
        {
            _dodgeTimer -= (float)delta;
            velocity.X = _dodgeDirection.X * DODGE_SPEED;
            velocity.Z = _dodgeDirection.Z * DODGE_SPEED;
            if (_dodgeTimer <= 0)
            {
                _isDodging = false;
                _isInvincible = false;
            }
        }
        else
        {
            // Normal movement
            float speed = (_stats?.Speed ?? 8f) * 1.0f;
            Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
            Vector3 moveDir = Vector3.Zero;

            if (_camera != null)
            {
                moveDir = (_camera.ForwardDirection * -inputDir.Y + _camera.RightDirection * inputDir.X).Normalized();
            }
            else
            {
                moveDir = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
            }

            if (moveDir.LengthSquared() > 0.01f)
            {
                velocity.X = moveDir.X * speed;
                velocity.Z = moveDir.Z * speed;

                // Mesh zur Bewegungsrichtung drehen — nur Y-Achse!
                // Atan2(X, Z) gibt den korrekten Winkel für KayKit (+Z = vorwärts)
                var lookDir = new Vector3(moveDir.X, 0f, moveDir.Z).Normalized();
                if (_characterMesh != null && lookDir.LengthSquared() > 0.001f)
                {
                    float targetAngle = Mathf.Atan2(-lookDir.X, -lookDir.Z);
                    float currentAngle = _characterMesh.GlobalRotation.Y;
                    float smoothAngle = Mathf.LerpAngle(currentAngle, targetAngle, 0.2f);
                    var gr = _characterMesh.GlobalRotation;
                    _characterMesh.GlobalRotation = new Vector3(gr.X, smoothAngle, gr.Z);
                }
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, 0f, 0.2f);
                velocity.Z = Mathf.Lerp(velocity.Z, 0f, 0.2f);
            }

            // Block
            IsBlocking = Input.IsActionPressed("block", true);

            // Dodge
            if (Input.IsActionJustPressed("dodge") && !(_combat?.IsAttacking ?? false))
            {
                StartDodge(moveDir.LengthSquared() > 0.01f ? moveDir : -Transform.Basis.Z);
            }

            // Lock-On toggle (Tab)
            if (Input.IsActionJustPressed("lock_on") && _camera != null)
            {
                if (_camera.GetLockOnTarget() != null)
                {
                    _camera.ClearLockOn();
                }
                else
                {
                    var target = FindNearestEnemy();
                    if (target != null) _camera.SetLockOnTarget(target);
                }
            }
        }

        // Hit incoming timer
        if (_hitIncomingTimer > 0)
        {
            _hitIncomingTimer -= (float)delta;
            if (_hitIncomingTimer <= 0) _hitIncoming = false;
        }

        Velocity = velocity;
        MoveAndSlide();

        // Lock-On: Mesh zum Ziel drehen
        var lockTarget = _camera?.GetLockOnTarget();
        if (lockTarget != null && IsInstanceValid(lockTarget) && _characterMesh != null)
        {
            var dir = (lockTarget.GlobalPosition - GlobalPosition);
            dir.Y = 0;
            if (dir.LengthSquared() > 0.01f)
            {
                var d = dir.Normalized();
                float lockAngle = Mathf.Atan2(-d.X, -d.Z);
                float curAngle = _characterMesh.GlobalRotation.Y;
                var lgr = _characterMesh.GlobalRotation;
                _characterMesh.GlobalRotation = new Vector3(lgr.X, Mathf.LerpAngle(curAngle, lockAngle, 0.25f), lgr.Z);
            }
        }

        UpdateAnimation(velocity);
    }

    private void StartDodge(Vector3 direction)
    {
        // Check for dodge counter (dodge within window of incoming hit)
        if (_hitIncoming)
        {
            EmitSignal(SignalName.DodgeCounter);
            _dataGauge?.AddGauge(DataGauge.DODGE_COUNTER);
        }

        _isDodging = true;
        _isInvincible = true;
        _dodgeTimer = DODGE_DURATION;
        _dodgeDirection = direction.Normalized();
        EmitSignal(SignalName.DodgeStarted);
        AudioManager.Instance?.PlaySfx(_hitIncoming ? "dodge_counter" : "dodge", GlobalPosition);
    }

    // Call this when an enemy is about to hit the player (e.g. from EnemyAI)
    public void NotifyIncomingHit(float windowSeconds = 0.2f)
    {
        _hitIncoming = true;
        _hitIncomingTimer = windowSeconds;
    }

    private Node3D FindNearestEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("enemies");
        Node3D nearest = null;
        float nearestDist = 20f; // Max lock-on range
        foreach (var e in enemies)
        {
            if (e is Node3D enemy3D && IsInstanceValid(enemy3D))
            {
                float dist = GlobalPosition.DistanceTo(enemy3D.GlobalPosition);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy3D;
                }
            }
        }
        return nearest;
    }

    private void UpdateAnimation(Vector3 velocity)
    {
        if (_animPlayer == null) return;
        bool isAttacking = _combat?.IsAttacking ?? false;
        if (_isDodging)       { PlayAnim("Dodge_Forward"); return; }
        if (isAttacking)      { return; } // combat handles attack anims
        float speed = new Vector2(velocity.X, velocity.Z).Length();
        if (speed > 5f)       PlayAnim("Running_A");
        else if (speed > 0.2f) PlayAnim("Walking_A");
        else                  PlayAnim("Idle");
    }

    private void PlayAnim(string name)
    {
        if (_animPlayer == null) return;
        if (_animPlayer.CurrentAnimation != name)
            _animPlayer.Play(name);
    }
}
