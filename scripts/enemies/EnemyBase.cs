using Godot;

public partial class EnemyBase : CharacterBody3D
{
    [Signal] public delegate void DiedEventHandler(EnemyBase enemy);
    [Signal] public delegate void DamageTakenEventHandler(float damage);

    [Export] public float MaxHp = 30f;
    [Export] public float Atk = 5f;
    [Export] public float Def = 2f;
    [Export] public float Spd = 4f;
    [Export] public string SkillDropId = "common_physical_strength";

    public float CurrentHp { get; protected set; }
    public bool IsDead { get; private set; } = false;

    public override void _Ready()
    {
        CurrentHp = MaxHp;
    }

    public virtual void TakeDamage(float damage, bool isBackAttack = false)
    {
        if (IsDead) return;

        float actualDamage = Mathf.Max(1f, damage - Def);
        if (isBackAttack) actualDamage *= 2f;

        CurrentHp -= actualDamage;
        EmitSignal(SignalName.DamageTaken, actualDamage);

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        EmitSignal(SignalName.Died, this);

        // Notify player combat for absorption
        var playerCombat = GetTree().Root.FindChild("PlayerCombat", true, false) as PlayerCombat;
        playerCombat?.OnEnemyKilled(this);

        // Remove after short delay
        var timer = new Timer();
        timer.WaitTime = 0.3f;
        timer.OneShot = true;
        timer.Timeout += () => QueueFree();
        AddChild(timer);
        timer.Start();
    }

    public float GetHpPercent() => CurrentHp / MaxHp;
}
