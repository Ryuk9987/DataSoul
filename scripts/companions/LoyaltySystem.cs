using Godot;
using System.Collections.Generic;

public partial class LoyaltySystem : Node
{
    public static LoyaltySystem Instance { get; private set; }

    [Signal] public delegate void LoyaltyChangedEventHandler(string companionId, int newValue);

    private Dictionary<string, int> _loyalty = new Dictionary<string, int>();

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        _loyalty["lyra"] = 50;
    }

    public void AddLoyalty(string companionId, int amount)
    {
        if (!_loyalty.ContainsKey(companionId)) _loyalty[companionId] = 50;
        _loyalty[companionId] = Mathf.Clamp(_loyalty[companionId] + amount, 0, 100);
        EmitSignal(SignalName.LoyaltyChanged, companionId, _loyalty[companionId]);
        GD.Print($"[Loyalty] {companionId}: {_loyalty[companionId]}/100");
    }

    public int GetLoyalty(string companionId)
        => _loyalty.TryGetValue(companionId, out int v) ? v : 50;

    public string GetLoyaltyTier(string companionId)
    {
        int val = GetLoyalty(companionId);
        if (val < 30) return "low";
        if (val > 60) return "high";
        return "neutral";
    }
}
