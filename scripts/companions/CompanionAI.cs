using Godot;
using System.Collections.Generic;

public partial class CompanionAI : CompanionBase
{
    private PlayerStats _playerStats;
    private bool _reviveUsed = false;

    // Cooldowns in seconds
    private Dictionary<string, float> _cooldowns = new Dictionary<string, float>
    {
        { "heal", 0f }, { "shield", 0f }, { "group_heal", 0f },
        { "emergency_heal", 0f }, { "cleanse", 0f }, { "barrier", 0f }
    };

    // Shield state
    private bool _shieldActive = false;
    private float _shieldTimer = 0f;
    private float _shieldAbsorb = 0f;

    public override void _Ready()
    {
        base._Ready();
        AddToGroup("companions");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // Tick cooldowns
        var keys = new List<string>(_cooldowns.Keys);
        foreach (var key in keys)
            if (_cooldowns[key] > 0) _cooldowns[key] -= (float)delta;

        // Shield timer
        if (_shieldActive)
        {
            _shieldTimer -= (float)delta;
            if (_shieldTimer <= 0) _shieldActive = false;
        }

        // Find player stats lazily
        if (_playerStats == null)
            _playerStats = FollowTarget?.GetNodeOrNull<PlayerStats>("PlayerStats");
    }

    protected override void ExecuteAI()
    {
        if (_playerStats == null) return;

        float hpPercent = _playerStats.CurrentHealth / _playerStats.MaxHealth;
        int nearbyEnemies = CountNearbyEnemies();

        // Priority 1: Revive
        if (_playerStats.CurrentHealth <= 0 && !_reviveUsed)
        {
            DoRevive();
            return;
        }

        // Priority 2: Emergency heal (<20%)
        if (hpPercent < 0.2f && _cooldowns["emergency_heal"] <= 0)
        {
            DoEmergencyHeal();
            return;
        }

        // Priority 3: Cleanse debuff (placeholder — no debuff system yet)
        // if (playerHasDebuff && _cooldowns["cleanse"] <= 0) { DoClean(); return; }

        // Priority 4: Heal (<50%)
        if (hpPercent < 0.5f && _cooldowns["heal"] <= 0)
        {
            DoHeal();
            return;
        }

        // Priority 5: Shield (HP <70% AND 2+ enemies)
        if (hpPercent < 0.7f && nearbyEnemies >= 2 && !_shieldActive && _cooldowns["shield"] <= 0)
        {
            DoShield();
            return;
        }

        // Priority 6: Group heal (party avg <60% — simplified: just player for now)
        if (hpPercent < 0.6f && _cooldowns["group_heal"] <= 0)
        {
            DoGroupHeal();
            return;
        }
    }

    private void DoRevive()
    {
        _reviveUsed = true;
        _playerStats.Heal(_playerStats.MaxHealth * 0.3f);
        GD.Print("[Lyra] Revive! Player restored to 30% HP.");
    }

    private void DoEmergencyHeal()
    {
        _cooldowns["emergency_heal"] = 25f;
        _playerStats.Heal(_playerStats.MaxHealth * 0.5f);
        GD.Print("[Lyra] Emergency Heal!");
    }

    private void DoHeal()
    {
        _cooldowns["heal"] = 8f;
        _playerStats.Heal(_playerStats.MaxHealth * 0.3f);
        GD.Print("[Lyra] Heal!");
    }

    private void DoShield()
    {
        _cooldowns["shield"] = 15f;
        _shieldActive = true;
        _shieldTimer = 8f;
        _shieldAbsorb = _playerStats.MaxHealth * 0.2f;
        GD.Print("[Lyra] Shield activated!");
        // TODO: Hook into PlayerStats.TakeDamage to absorb damage
    }

    private void DoGroupHeal()
    {
        _cooldowns["group_heal"] = 20f;
        _playerStats.Heal(_playerStats.MaxHealth * 0.2f);
        GD.Print("[Lyra] Group Heal!");
    }

    private int CountNearbyEnemies()
    {
        int count = 0;
        var enemies = GetTree().GetNodesInGroup("enemies");
        foreach (Node3D enemy in enemies)
        {
            if (FollowTarget != null &&
                FollowTarget.GlobalPosition.DistanceTo(enemy.GlobalPosition) < 10f)
                count++;
        }
        return count;
    }

    public void ResetForNewCombat()
    {
        _reviveUsed = false;
    }
}
