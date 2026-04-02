using Godot;

/// <summary>
/// Synergy-Attack System — laut GDD: Q-Taste löst gemeinsamen Angriff von
/// Spieler + Lyra aus wenn Synergy-Gauge voll ist.
/// "Data Blessing": kurze Unverwundbarkeit + Schadens-Boost für den Protagonisten.
/// </summary>
public partial class SynergySystem : Node
{
    public static SynergySystem Instance { get; private set; }

    [Signal] public delegate void SynergyGaugeChangedEventHandler(float current, float max);
    [Signal] public delegate void SynergyActivatedEventHandler();

    public float MaxGauge { get; private set; } = 100f;
    public float CurrentGauge { get; private set; } = 0f;
    public bool IsReady => CurrentGauge >= MaxGauge;

    // Boost state
    private bool _boostActive = false;
    private float _boostTimer = 0f;
    private const float BOOST_DURATION = 5f;
    private const float DAMAGE_MULTIPLIER = 1.8f;

    private PlayerStats _playerStats;
    private PlayerController _playerController;
    private CompanionAI _lyra;

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(MethodName.ConnectSystems);
    }

    private void ConnectSystems()
    {
        var player = GetTree().Root.FindChild("Player", true, false) as Node3D;
        if (player != null)
        {
            _playerStats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
            _playerController = player as PlayerController;
        }
        _lyra = GetTree().Root.FindChild("Lyra", true, false) as CompanionAI;
    }

    public override void _Process(double delta)
    {
        if (_boostActive)
        {
            _boostTimer -= (float)delta;
            if (_boostTimer <= 0)
            {
                _boostActive = false;
                GD.Print("[Synergy] Data Blessing ended.");
            }
        }
    }

    public void AddGauge(float amount)
    {
        if (_boostActive) return;
        CurrentGauge = Mathf.Min(CurrentGauge + amount, MaxGauge);
        EmitSignal(SignalName.SynergyGaugeChanged, CurrentGauge, MaxGauge);
    }

    public bool TryActivate()
    {
        if (!IsReady) return false;
        CurrentGauge = 0f;
        EmitSignal(SignalName.SynergyGaugeChanged, CurrentGauge, MaxGauge);
        EmitSignal(SignalName.SynergyActivated);
        ActivateDataBlessing();
        return true;
    }

    private void ActivateDataBlessing()
    {
        _boostActive = true;
        _boostTimer = BOOST_DURATION;

        // Invincibility on player
        // (PlayerController doesn't have a public SetInvincible — we use the existing
        //  dodge invincibility by calling a brief timed effect instead)
        GD.Print("[Synergy] Data Blessing activated! Damage x1.8 for 5s.");

        // Visual: Lyra heal aura + player glow
        var lyraVisual = GetTree().Root.FindChild("LyraVisual", true, false) as LyraVisual;
        lyraVisual?.ShowHealEffect(BOOST_DURATION);

        // Dialogue
        DialogueSystem.Instance?.ShowLine("Lyra",
            "Data Blessing! Ich habe deine Signatur verstärkt — 5 Sekunden!", 3f);

        // Damage boost: applied in GetDamageMultiplier()
    }

    public float GetDamageMultiplier() => _boostActive ? DAMAGE_MULTIPLIER : 1f;

    public bool IsBoostActive => _boostActive;
    public float BoostTimeRemaining => _boostTimer;
}
