using Godot;

public partial class HUDController : CanvasLayer
{
    [Export] public NodePath HPBarPath;
    [Export] public NodePath DataGaugeBarPath;
    [Export] public NodePath CompanionHPBarPath;
    [Export] public NodePath AbsorptionPopupPath;
    [Export] public NodePath OverloadWarningPath;
    [Export] public NodePath LevelLabelPath;

    private ProgressBar _hpBar;
    private ProgressBar _dataGaugeBar;
    private ProgressBar _companionHpBar;
    private Label _absorptionPopup;
    private ColorRect _overloadWarning;
    private Label _levelLabel;
    private Timer _popupTimer;

    // Skill slot refs (4 panels)
    private Control[] _skillSlots = new Control[4];
    private Label[] _cooldownLabels = new Label[4];

    public override void _Ready()
    {
        _hpBar = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/HPBar");
        _dataGaugeBar = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/DataGaugeBar");
        _companionHpBar = GetNodeOrNull<ProgressBar>("CompanionHP");
        _absorptionPopup = GetNodeOrNull<Label>("AbsorptionPopup");
        _overloadWarning = GetNodeOrNull<ColorRect>("OverloadWarning");
        _levelLabel = GetNodeOrNull<Label>("MarginContainer/TopLeft/LevelLabel");

        for (int i = 0; i < 4; i++)
        {
            _skillSlots[i] = GetNodeOrNull<Control>($"SkillBar/Slot{i + 1}");
            _cooldownLabels[i] = GetNodeOrNull<Label>($"SkillBar/Slot{i + 1}/CooldownLabel");
        }

        _popupTimer = new Timer();
        _popupTimer.WaitTime = 2.5f;
        _popupTimer.OneShot = true;
        _popupTimer.Timeout += () => { if (_absorptionPopup != null) _absorptionPopup.Visible = false; };
        AddChild(_popupTimer);

        // Hide warnings initially
        if (_overloadWarning != null) _overloadWarning.Visible = false;
        if (_absorptionPopup != null) _absorptionPopup.Visible = false;

        // Connect to player stats
        CallDeferred(MethodName.ConnectToPlayer);
    }

    private void ConnectToPlayer()
    {
        var player = GetTree().Root.FindChild("Player", true, false);
        if (player == null) return;

        var stats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
        stats?.Connect(PlayerStats.SignalName.HealthChanged, new Callable(this, MethodName.UpdateHP));

        var gauge = player.GetNodeOrNull<DataGauge>("DataGauge");
        gauge?.Connect(DataGauge.SignalName.GaugeChanged, new Callable(this, MethodName.UpdateDataGauge));

        var absorption = player.GetNodeOrNull<DataAbsorption>("DataAbsorption");
        absorption?.Connect(DataAbsorption.SignalName.AbsorptionComplete,
            new Callable(this, MethodName.OnAbsorptionComplete));
    }

    public void UpdateHP(float current, float max)
    {
        if (_hpBar == null) return;
        _hpBar.MaxValue = max;
        _hpBar.Value = current;
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

    public void ShowAbsorptionPopup(string skillName)
    {
        if (_absorptionPopup == null) return;
        _absorptionPopup.Text = $"[{skillName}] absorbed!";
        _absorptionPopup.Visible = true;
        _popupTimer.Start();
    }

    private void OnAbsorptionComplete(string skillId, int newLevel)
    {
        ShowAbsorptionPopup($"{skillId} Lv{newLevel}");
    }

    public void ShowOverloadWarning(bool active)
    {
        if (_overloadWarning != null) _overloadWarning.Visible = active;
    }

    public void UpdateSkillCooldown(int slot, float remaining, float total)
    {
        if (slot < 0 || slot >= 4 || _cooldownLabels[slot] == null) return;
        if (remaining > 0)
            _cooldownLabels[slot].Text = Mathf.Ceil(remaining).ToString();
        else
            _cooldownLabels[slot].Text = "";
    }

    public void UpdateLevel(int level)
    {
        if (_levelLabel != null) _levelLabel.Text = $"LVL {level}";
    }
}
