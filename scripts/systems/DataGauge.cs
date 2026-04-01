using Godot;
using System;

public partial class DataGauge : Node
{
    [Signal] public delegate void GaugeChangedEventHandler(float current, float max);

    [Export] public float MaxGauge { get; set; } = 100;
    public float CurrentGauge { get; private set; } = 0;

    public const int LIGHT_HIT = 5;
    public const int HEAVY_HIT = 15;
    public const int DODGE_COUNTER = 25;
    public const int KILL = 30;

    public override void _Ready()
    {
        // Initialization if needed
    }

    public void AddGauge(int amount)
    {
        CurrentGauge = Mathf.Min(CurrentGauge + amount, MaxGauge);
        EmitSignal(SignalName.GaugeChanged, CurrentGauge, MaxGauge);
    }

    public void UseGauge(int amount)
    {
        CurrentGauge = Mathf.Max(CurrentGauge - amount, 0);
        EmitSignal(SignalName.GaugeChanged, CurrentGauge, MaxGauge);
    }

    public void Reset()
    {
        CurrentGauge = 0;
        EmitSignal(SignalName.GaugeChanged, CurrentGauge, MaxGauge);
    }
}