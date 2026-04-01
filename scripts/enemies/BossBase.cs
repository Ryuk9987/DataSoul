using Godot;
using System.Collections.Generic;

public partial class BossBase : EnemyBase
{
    [Signal] public delegate void PhaseChangedEventHandler(int newPhase);

    [Export] public float[] PhaseThresholds = { 0.6f, 0.3f }; // HP% triggers

    public int CurrentPhase { get; private set; } = 1;
    private int _nextThresholdIndex = 0;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void TakeDamage(float damage, bool isBackAttack = false)
    {
        base.TakeDamage(damage, isBackAttack);
        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        if (_nextThresholdIndex >= PhaseThresholds.Length) return;

        float hpPercent = GetHpPercent();
        if (hpPercent <= PhaseThresholds[_nextThresholdIndex])
        {
            CurrentPhase++;
            _nextThresholdIndex++;
            EmitSignal(SignalName.PhaseChanged, CurrentPhase);
            OnPhaseChange(CurrentPhase);
        }
    }

    protected virtual void OnPhaseChange(int newPhase)
    {
        // Override in subclasses
    }
}
