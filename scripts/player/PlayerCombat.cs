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
    private const float ATTACK_DURATION = 0.4f;

    // Hitbox area
    private Area3D _hitbox;
    private List<Node3D> _hitThisAttack = new List<Node3D>();

    public bool IsAttacking => _isAttacking;

    public override void _Ready()
    {
        _stats = GetParent().GetNodeOrNull<PlayerStats>("PlayerStats");
        _dataGauge = GetParent().GetNodeOrNull<DataGauge>("DataGauge");
        _absorption = GetParent().GetNodeOrNull<DataAbsorption>("DataAbsorption");
        _controller = GetParent() as PlayerController;
        _hitbox = GetParent().GetNodeOrNull<Area3D>("HitboxArea");
    }

    public override void _Process(double delta)
    {
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

        // Input — can't attack during dodge
        if (_controller?.IsInvincible ?? false) return;

        bool lightPressed = Input.IsActionJustPressed("attack_light");
        bool heavyPressed = Input.IsActionJustPressed("attack_heavy");

        // Dodge cancel: allow input during attack (except last hit of combos)
        bool canInput = !_isAttacking || _currentCombo != ComboState.Light3;

        if (!canInput) return;

        if (lightPressed) HandleLightAttack();
        else if (heavyPressed) HandleHeavyAttack();

        // Skill inputs
        for (int i = 0; i < 4; i++)
        {
            if (Input.IsActionJustPressed($"skill_{i + 1}"))
                GetParent().GetNodeOrNull<SkillSystem>("SkillSystem")?.UseSkill(i);
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

        // Apply damage to enemies in hitbox
        if (_hitbox != null)
        {
            foreach (var body in _hitbox.GetOverlappingBodies())
            {
                if (body is EnemyBase enemy && !_hitThisAttack.Contains(enemy))
                {
                    _hitThisAttack.Add(enemy);

                    // Back attack check
                    bool isBackAttack = false;
                    if (_controller != null)
                    {
                        var toEnemy = (enemy.GlobalPosition - _controller.GlobalPosition).Normalized();
                        isBackAttack = toEnemy.Dot(-enemy.GlobalTransform.Basis.Z) > 0.5f;
                    }

                    enemy.TakeDamage(baseDamage, isBackAttack);
                    _dataGauge?.AddGauge(isHeavy ? DataGauge.HEAVY_HIT : DataGauge.LIGHT_HIT);
                    EmitSignal(SignalName.HitLanded, enemy, baseDamage, isHeavy);
                }
            }
        }
    }

    public void OnEnemyKilled(EnemyBase enemy)
    {
        _dataGauge?.AddGauge(DataGauge.KILL);
        _absorption?.StartAbsorption(enemy);
        _currentCombo = ComboState.None;
    }
}
