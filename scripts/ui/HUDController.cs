using Godot;

public partial class HUDController : CanvasLayer
{
    private ProgressBar _hpBar;
    private ProgressBar _dataGaugeBar;
    private ProgressBar _companionHpBar;
    private Label _absorptionPopup;
    private ColorRect _overloadWarning;
    private Label _levelLabel;
    private Label _companionLabel;
    private Timer _popupTimer;

    private Label[] _cooldownLabels = new Label[4];
    private Label[] _slotLabels = new Label[4];

    private PlayerStats _playerStats;
    private DataGauge _dataGauge;
    private DataAbsorption _absorption;
    private PlayerCombat _combat;

    public override void _Ready()
    {
        // Get HUD nodes
        _hpBar = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/HPBar");
        _dataGaugeBar = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/DataGaugeBar");
        _levelLabel = GetNodeOrNull<Label>("MarginContainer/TopLeft/LevelLabel");
        _companionHpBar = GetNodeOrNull<ProgressBar>("CompanionHP");
        _companionLabel = GetNodeOrNull<Label>("CompanionLabel");
        _absorptionPopup = GetNodeOrNull<Label>("AbsorptionPopup");
        _overloadWarning = GetNodeOrNull<ColorRect>("OverloadWarning");

        for (int i = 0; i < 4; i++)
        {
            _slotLabels[i] = GetNodeOrNull<Label>($"SkillBar/Slot{i + 1}/SlotLabel");
            _cooldownLabels[i] = GetNodeOrNull<Label>($"SkillBar/Slot{i + 1}/CooldownLabel");
        }

        // Label for skill 1 = Code-Injection
        if (_slotLabels[0] != null) _slotLabels[0].Text = "INJ";

        _popupTimer = new Timer();
        _popupTimer.WaitTime = 2.5f;
        _popupTimer.OneShot = true;
        _popupTimer.Timeout += () => { if (_absorptionPopup != null) _absorptionPopup.Visible = false; };
        AddChild(_popupTimer);

        if (_overloadWarning != null) _overloadWarning.Visible = false;
        if (_absorptionPopup != null) _absorptionPopup.Visible = false;

        // Connect after one frame so Player is ready
        CallDeferred(MethodName.ConnectToPlayer);
    }

    public override void _Process(double delta)
    {
        // Update skill cooldown display every frame
        if (_combat != null && _cooldownLabels[0] != null)
        {
            float remaining = _combat.GetSkillCooldownRemaining();
            _cooldownLabels[0].Text = remaining > 0 ? Mathf.Ceil(remaining).ToString() : "";
        }
    }

    private void ConnectToPlayer()
    {
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count == 0) return;

        var player = players[0] as Node3D;
        if (player == null) return;

        _playerStats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
        _dataGauge = player.GetNodeOrNull<DataGauge>("DataGauge");
        _absorption = player.GetNodeOrNull<DataAbsorption>("DataAbsorption");
        _combat = player.GetNodeOrNull<PlayerCombat>("PlayerCombat");

        if (_playerStats != null)
        {
            _playerStats.HealthChanged += UpdateHP;
            UpdateHP(_playerStats.CurrentHealth, _playerStats.MaxHealth);
        }

        if (_dataGauge != null)
        {
            _dataGauge.GaugeChanged += UpdateDataGauge;
            UpdateDataGauge(_dataGauge.CurrentGauge, _dataGauge.MaxGauge);
        }

        if (_absorption != null)
            _absorption.AbsorptionComplete += OnAbsorptionComplete;
    }

    public void UpdateHP(float current, float max)
    {
        if (_hpBar == null) return;
        _hpBar.MaxValue = max;
        _hpBar.Value = current;

        // Overload warning: red flash when HP < 20%
        ShowOverloadWarning(current / max < 0.2f);
    }

    public void UpdateDataGauge(float current, float max)
    {
        if (_dataGaugeBar == null) return;
        _dataGaugeBar.MaxValue = max;
        _dataGaugeBar.Value = current;
    }

    public void UpdateCompanionHP(float current, float max)
    {
        if (_companionHpBar == null) return;
        _companionHpBar.MaxValue = max;
        _companionHpBar.Value = current;
    }

    private void OnAbsorptionComplete(string skillId, int newLevel)
    {
        ShowAbsorptionPopup(skillId, newLevel);
    }

    public void ShowAbsorptionPopup(string skillId, int level)
    {
        if (_absorptionPopup == null) return;
        _absorptionPopup.Text = $"[{skillId}]  Lv{level}  absorbed!";
        _absorptionPopup.Visible = true;
        _popupTimer.Start();
    }

    public void ShowOverloadWarning(bool active)
    {
        if (_overloadWarning != null) _overloadWarning.Visible = active;
    }

    public void UpdateLevel(int level)
    {
        if (_levelLabel != null) _levelLabel.Text = $"LVL {level}";
    }
}
