using Godot;

/// <summary>
/// Corruption Overload-Mechanik.
/// Wenn 3+ Corrupted Skills aktiv → gelegentliche Fehlzündungen (Selbstschaden).
/// Bei 4+ steigt die Wahrscheinlichkeit deutlich an.
/// </summary>
public partial class CorruptionSystem : Node
{
    public static CorruptionSystem Instance { get; private set; }

    private const float DamagePer_Misfire  = 5f;
    private const float ChanceThreshold3   = 0.05f;   // 5%  pro Frame bei Count == 3
    private const float ChanceThreshold4   = 0.15f;   // 15% pro Frame bei Count >= 4

    private PlayerStats _playerStats;
    private HUDController _hud;

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(MethodName.ConnectSystems);
    }

    private void ConnectSystems()
    {
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count > 0)
        {
            var player = players[0] as Node3D;
            _playerStats = player?.GetNodeOrNull<PlayerStats>("PlayerStats");
        }

        _hud = GetTree().Root.FindChild("PlayerHUD", true, false) as HUDController;
    }

    /// <summary>
    /// Zählt aktive Corrupted Skills in den aktiven Slots des SkillSystem.
    /// Corrupted Skills haben IDs die mit "corrupted_" beginnen.
    /// </summary>
    public int GetCorruptedCount()
    {
        var skillSystem = GetTree().Root.FindChild("SkillSystem", true, false) as SkillSystem;
        if (skillSystem == null) return 0;

        int count = 0;
        for (int i = 0; i < skillSystem.ActiveSlotCount; i++)
        {
            string skillId = skillSystem.GetActiveSkill(i);
            if (!string.IsNullOrEmpty(skillId) && skillId.StartsWith("corrupted_"))
                count++;
        }
        return count;
    }

    public override void _Process(double delta)
    {
        int count = GetCorruptedCount();

        // HUD OverloadWarning bei Corruption >= 3 aktiv
        UpdateOverloadWarning(count);

        if (count < 3) return;

        // Fehlzündungs-Chance
        float chance = count >= 4 ? ChanceThreshold4 : ChanceThreshold3;

        // Per-Frame Random Check (abhängig von delta skaliert auf ~60fps Basis)
        float scaledChance = chance * (float)delta * 60f;
        if (GD.Randf() < scaledChance)
        {
            TriggerMisfire();
        }
    }

    private void TriggerMisfire()
    {
        if (_playerStats == null)
        {
            // Lazy re-connect
            var players = GetTree().GetNodesInGroup("player");
            if (players.Count > 0)
                _playerStats = (players[0] as Node3D)?.GetNodeOrNull<PlayerStats>("PlayerStats");
        }

        _playerStats?.TakeDamage(DamagePer_Misfire);
        DialogueSystem.Instance?.ShowLine(
            "System",
            "CORRUPTION OVERLOAD — Signatur destabilisiert!",
            2f);
    }

    private void UpdateOverloadWarning(int corruptedCount)
    {
        if (_hud == null)
        {
            _hud = GetTree().Root.FindChild("PlayerHUD", true, false) as HUDController;
            if (_hud == null) return;
        }

        if (corruptedCount >= 3)
        {
            // Direkt die interne Methode imitieren — ShowOverload über TakeDamage-Umweg
            // wäre falsch, daher über das HUD-Interface:
            // HUDController.UpdateHP setzt OverloadWarning basierend auf HP-Prozent.
            // Wir setzen sie hier manuell über die öffentliche Methode.
            _hud.SetCorruptionOverload(true);
        }
        else
        {
            _hud.SetCorruptionOverload(false);
        }
    }
}
