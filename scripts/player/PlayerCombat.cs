using Godot;
using System.Collections.Generic;

public partial class PlayerCombat : Node
{
    [Signal] public delegate void HitLandedEventHandler(Node3D target, float damage, bool isHeavy);
    [Signal] public delegate void ComboFinishedEventHandler();

    private PlayerStats _stats;
    private DataGauge _dataGauge;
    private DataAbsorption _absorption;
    private PlayerController _controller;

    // Combo state
    private enum ComboState { None, Light1, Light2, Light3, Heavy1, HeavyHeavy, LightLightHeavy }
    private ComboState _currentCombo = ComboState.None;
    private float _comboResetTimer = 0f;
    private const float COMBO_WINDOW = 0.8f;

    private bool _isAttacking = false;
    private float _attackTimer = 0f;
    private const float ATTACK_DURATION = 0.45f;

    // Hitbox: sphere cast in front of player
    private const float HITBOX_RANGE = 2.0f;
    private const float HITBOX_RADIUS = 1.0f;
    private List<Node3D> _hitThisAttack = new List<Node3D>();

    // Active skill: Code-Injection
    private float _skillCooldown = 0f;
    private const float SKILL_COOLDOWN = 12.0f;

    public bool IsAttacking => _isAttacking;

    public override void _Ready()
    {
        _stats = GetParent().GetNodeOrNull<PlayerStats>("PlayerStats");
        _dataGauge = GetParent().GetNodeOrNull<DataGauge>("DataGauge");
        _absorption = GetParent().GetNodeOrNull<DataAbsorption>("DataAbsorption");
        _controller = GetParent() as PlayerController;
    }

    public override void _Process(double delta)
    {
        // Tick cooldown
        if (_skillCooldown > 0) _skillCooldown -= (float)delta;

        // Combo reset timer
        if (_currentCombo != ComboState.None && !_isAttacking)
        {
            _comboResetTimer -= (float)delta;
            if (_comboResetTimer <= 0) _currentCombo = ComboState.None;
        }

        // Attack timer
        if (_isAttacking)
        {
            _attackTimer -= (float)delta;
            if (_attackTimer <= 0)
            {
                _isAttacking = false;
                _hitThisAttack.Clear();
                EmitSignal(SignalName.ComboFinished);
            }
        }

        // Don't accept input during dodge
        if (_controller?.IsInvincible ?? false) return;

        // Dodge-cancel: allow new input except during last combo hit
        bool canInput = !_isAttacking || (_currentCombo != ComboState.Light3
            && _currentCombo != ComboState.LightLightHeavy
            && _currentCombo != ComboState.HeavyHeavy);

        if (!canInput) return;

        bool lightPressed = Input.IsActionJustPressed("attack_light");
        bool heavyPressed = Input.IsActionJustPressed("attack_heavy");

        if (lightPressed) HandleLightAttack();
        else if (heavyPressed) HandleHeavyAttack();

        // Skill 1: Code-Injection
        if (InputMap.HasAction("skill_1") && Input.IsActionJustPressed("skill_1"))
            UseCodeInjection();

        // Synergy-Attack: Q
        if (InputMap.HasAction("synergy") && Input.IsActionJustPressed("synergy"))
        {
            if (SynergySystem.Instance != null && SynergySystem.Instance.IsReady)
                SynergySystem.Instance.TryActivate();
            else
                DialogueSystem.Instance?.ShowLine("Lyra", "Die Synergy-Gauge ist noch nicht voll.", 2f);
        }
    }

    private void HandleLightAttack()
    {
        switch (_currentCombo)
        {
            case ComboState.None:
                ExecuteAttack(ComboState.Light1, 1.0f, false);
                break;
            case ComboState.Light1:
                ExecuteAttack(ComboState.Light2, 1.2f, false);
                break;
            case ComboState.Light2:
                ExecuteAttack(ComboState.Light3, 1.5f, false, isAoE: true);
                break;
        }
    }

    private void HandleHeavyAttack()
    {
        switch (_currentCombo)
        {
            case ComboState.None:
                ExecuteAttack(ComboState.Heavy1, 2.0f, true, knockback: true);
                break;
            case ComboState.Light2:
                ExecuteAttack(ComboState.LightLightHeavy, 2.5f, true, stagger: true);
                break;
            case ComboState.Heavy1:
                ExecuteAttack(ComboState.HeavyHeavy, 1.8f, true, isAoE: true);
                break;
        }
    }

    private void ExecuteAttack(ComboState newState, float multiplier, bool isHeavy,
        bool knockback = false, bool stagger = false, bool isAoE = false)
    {
        _currentCombo = newState;
        _isAttacking = true;
        _attackTimer = ATTACK_DURATION;
        _comboResetTimer = COMBO_WINDOW;
        _hitThisAttack.Clear();

        float baseDamage = (_stats?.Attack ?? 10f) * multiplier;
        var origin = _controller?.GlobalPosition ?? Vector3.Zero;
        var forward = -(_controller?.Transform.Basis.Z ?? Vector3.Forward);

        // Sphere cast in front of player
        var spaceState = _controller?.GetWorld3D()?.DirectSpaceState;
        if (spaceState == null) return;

        float radius = isAoE ? HITBOX_RADIUS * 1.8f : HITBOX_RADIUS;
        var center = origin + Vector3.Up * 0.9f + forward * (HITBOX_RANGE * 0.5f);

        var shape = new SphereShape3D();
        shape.Radius = radius;

        var query = new PhysicsShapeQueryParameters3D();
        query.Shape = shape;
        query.Transform = new Transform3D(Basis.Identity, center);
        query.CollisionMask = 0b11; // layers 1+2

        var results = spaceState.IntersectShape(query, 8);
        foreach (var result in results)
        {
            var collider = result["collider"].As<Node3D>();
            if (collider == null || _hitThisAttack.Contains(collider)) continue;
            if (collider == _controller) continue;

            if (collider is EnemyBase enemy)
            {
                _hitThisAttack.Add(enemy);

                // Back attack check
                bool isBackAttack = false;
                var toEnemy = (enemy.GlobalPosition - origin).Normalized();
                isBackAttack = toEnemy.Dot(-enemy.GlobalTransform.Basis.Z) > 0.5f;

                enemy.TakeDamage(baseDamage, isBackAttack);
                _dataGauge?.AddGauge(isHeavy ? DataGauge.HEAVY_HIT : DataGauge.LIGHT_HIT);
                SynergySystem.Instance?.AddGauge(isHeavy ? 8f : 4f);
                EmitSignal(SignalName.HitLanded, enemy, baseDamage, isHeavy);
            }
        }
    }

    // Code-Injection: DoT + DEF reduction (Programmierer start skill)
    private void UseCodeInjection()
    {
        if (_skillCooldown > 0) return;
        _skillCooldown = SKILL_COOLDOWN;

        var origin = _controller?.GlobalPosition ?? Vector3.Zero;
        var spaceState = _controller?.GetWorld3D()?.DirectSpaceState;
        if (spaceState == null) return;

        var shape = new SphereShape3D();
        shape.Radius = 4.0f;
        var query = new PhysicsShapeQueryParameters3D();
        query.Shape = shape;
        query.Transform = new Transform3D(Basis.Identity, origin + Vector3.Up * 0.9f);
        query.CollisionMask = 0b11;

        var results = spaceState.IntersectShape(query, 4);
        foreach (var result in results)
        {
            var collider = result["collider"].As<Node3D>();
            if (collider is EnemyBase enemy)
            {
                // Apply DoT for 6 seconds
                var dotTimer = new Timer();
                dotTimer.WaitTime = 1.0f;
                dotTimer.Autostart = true;
                float dotDuration = 6.0f;
                float elapsed = 0f;
                float dotDamage = 3f + (_stats != null ? (_stats.Attack - 10f) * 0.5f : 0f);

                dotTimer.Timeout += () =>
                {
                    elapsed += 1f;
                    if (!IsInstanceValid(enemy) || enemy.IsDead) { dotTimer.QueueFree(); return; }
                    enemy.TakeDamage(dotDamage, false);
                    if (elapsed >= dotDuration) dotTimer.QueueFree();
                };
                _controller?.AddChild(dotTimer);
            }
        }

        GD.Print("[Code-Injection] activated!");
    }

    public void OnEnemyKilled(EnemyBase enemy)
    {
        _dataGauge?.AddGauge(DataGauge.KILL);
        _absorption?.StartAbsorption(enemy);
        SynergySystem.Instance?.AddGauge(20f); // Kill = +20 Synergy
        _currentCombo = ComboState.None;
        _isAttacking = false;
    }

    public float GetSkillCooldownPercent() => _skillCooldown / SKILL_COOLDOWN;
    public float GetSkillCooldownRemaining() => _skillCooldown;
}
