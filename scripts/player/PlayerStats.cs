using Godot;
using System;

public partial class PlayerStats : Node
{
    public enum Background
    {
        Programmer,
        Gamer,
        Hacker,
        Creator,
        Analyst
    }

    [Signal] public delegate void HealthChangedEventHandler(float current, float max);
    [Signal] public delegate void DataGaugeChangedEventHandler(float current, float max);
    [Signal] public delegate void DiedEventHandler();

    [Export] public Background PlayerBackground { get; set; } = Background.Programmer;

    public float MaxHealth { get; private set; } = 100;
    public float CurrentHealth { get; private set; } = 100;
    public float Attack { get; private set; } = 10;
    public float Defense { get; private set; } = 5;
    public float Speed { get; private set; } = 8;
    public float DataCap { get; private set; } = 10;
    public float MaxDataGauge { get; private set; } = 100;
    public float CurrentDataGauge { get; private set; } = 0;

    public override void _Ready()
    {
        ApplyBackgroundBonuses();
    }

    private void ApplyBackgroundBonuses()
    {
        switch (PlayerBackground)
        {
            case Background.Programmer:
                Attack += 3;
                Speed += 2;
                break;
            case Background.Gamer:
                Attack += 5;
                break;
            case Background.Hacker:
                Speed += 4;
                break;
            case Background.Creator:
                MaxHealth += 10;
                CurrentHealth += 10;
                break;
            case Background.Analyst:
                Defense += 3;
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        float actualDamage = damage * (1 - Defense / 100);
        CurrentHealth -= actualDamage;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    public void AddDataGauge(float amount)
    {
        CurrentDataGauge = Mathf.Min(CurrentDataGauge + amount, MaxDataGauge);
        EmitSignal(SignalName.DataGaugeChanged, CurrentDataGauge, MaxDataGauge);
    }

    public void UseDataGauge(float amount)
    {
        CurrentDataGauge = Mathf.Max(CurrentDataGauge - amount, 0);
        EmitSignal(SignalName.DataGaugeChanged, CurrentDataGauge, MaxDataGauge);
    }

    public void ResetDataGauge()
    {
        CurrentDataGauge = 0;
        EmitSignal(SignalName.DataGaugeChanged, CurrentDataGauge, MaxDataGauge);
    }
}