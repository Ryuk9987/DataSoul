using Godot;

public partial class HUDController : CanvasLayer
{
    private ProgressBar _hpBar;
    private ProgressBar _xpBar;
    private ProgressBar _dataGaugeBar;
    private ProgressBar _companionHpBar;
    private ProgressBar _synergyBar;
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
    private LevelSystem _levelSystem;

    private float _overloadPulse = 0f;
    private bool _corruptionOverload = false;

    public override void _Ready()
    {
        _hpBar          = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/HPBar");
        _xpBar          = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/XPBar");
        _dataGaugeBar   = GetNodeOrNull<ProgressBar>("MarginContainer/TopLeft/DataGaugeBar");
        _levelLabel     = GetNodeOrNull<Label>("MarginContainer/TopLeft/LevelLabel");
        _companionHpBar = GetNodeOrNull<ProgressBar>("CompanionHP");
        _companionLabel = GetNodeOrNull<Label>("CompanionLabel");
        _absorptionPopup= GetNodeOrNull<Label>("AbsorptionPopup");
        _overloadWarning= GetNodeOrNull<ColorRect>("OverloadWarning");
        _synergyBar     = GetNodeOrNull<ProgressBar>("SynergyGauge/SynergyBar");

        for (int i = 0; i < 4; i++)
        {
            _slotLabels[i]    = GetNodeOrNull<Label>($"SkillBar/Slot{i + 1}/SlotLabel");
            _cooldownLabels[i]= GetNodeOrNull<Label>($"SkillBar/Slot{i + 1}/CooldownLabel");
        }

        if (_slotLabels[0] != null) _slotLabels[0].Text = "INJ";

        _popupTimer = new Timer();
        _popupTimer.WaitTime = 3f;
        _popupTimer.OneShot = true;
        _popupTimer.Timeout += () => { if (_absorptionPopup != null) _absorptionPopup.Visible = false; };
        AddChild(_popupTimer);

        if (_overloadWarning != null) _overloadWarning.Visible = false;
        if (_absorptionPopup != null) _absorptionPopup.Visible = false;

        CallDeferred(MethodName.ConnectToPlayer);
    }

    public override void _Process(double delta)
    {
        // Skill cooldown display
        if (_combat != null && _cooldownLabels[0] != null)
        {
            float rem = _combat.GetSkillCooldownRemaining();
            _cooldownLabels[0].Text = rem > 0 ? Mathf.Ceil(rem).ToString() : "";
        }

        // Overload warning pulse (sine wave alpha)
        if (_overloadWarning != null && _overloadWarning.Visible)
        {
            _overloadPulse += (float)delta * 3f;
            float alpha = 0.08f + 0.12f * Mathf.Sin(_overloadPulse);
            _overloadWarning.Color = new Color(1f, 0f, 0f, alpha);
        }

        // Lock-On indicator: position a Label over the locked enemy
        UpdateLockOnIndicator();
    }

    private void ConnectToPlayer()
    {
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count == 0) return;

        var player = players[0] as Node3D;
        if (player == null) return;

        _playerStats  = player.GetNodeOrNull<PlayerStats>("PlayerStats");
        _dataGauge    = player.GetNodeOrNull<DataGauge>("DataGauge");
        _absorption   = player.GetNodeOrNull<DataAbsorption>("DataAbsorption");
        _combat       = player.GetNodeOrNull<PlayerCombat>("PlayerCombat");
        _levelSystem  = player.GetNodeOrNull<LevelSystem>("LevelSystem");

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

        if (_levelSystem != null)
        {
            _levelSystem.LevelUp += UpdateLevel;
            _levelSystem.XpGained += UpdateXP;
            UpdateLevel(_levelSystem.Level);
            if (_xpBar != null)
            {
                _xpBar.MaxValue = _levelSystem.XpForNextLevel;
                _xpBar.Value    = _levelSystem.CurrentXp;
            }
        }

        if (SynergySystem.Instance != null)
            SynergySystem.Instance.SynergyGaugeChanged += UpdateSynergyGauge;
    }

    /// <summary>
    /// Wird von CorruptionSystem aufgerufen um den Overload-Zustand
    /// unabhängig vom HP-Stand zu steuern.
    /// </summary>
    public void SetCorruptionOverload(bool active)
    {
        _corruptionOverload = active;
        RefreshOverloadWarning(_playerStats?.CurrentHealth ?? 100f, _playerStats?.MaxHealth ?? 100f);
    }

    public void UpdateHP(float current, float max)
    {
        if (_hpBar != null)
        {
            _hpBar.MaxValue = max;
            _hpBar.Value    = current;
        }
        RefreshOverloadWarning(current, max);
    }

    private void RefreshOverloadWarning(float current, float max)
    {
        bool lowHp     = max > 0 && (current / max) < 0.2f;
        bool overload  = lowHp || _corruptionOverload;

        if (_overloadWarning != null)
        {
            if (overload && !_overloadWarning.Visible)
            {
                _overloadPulse = 0f;
                _overloadWarning.Visible = true;
            }
            else if (!overload && _overloadWarning.Visible)
            {
                _overloadWarning.Visible = false;
            }
        }
    }

    public void UpdateDataGauge(float current, float max)
    {
        if (_dataGaugeBar == null) return;
        _dataGaugeBar.MaxValue = max;
        _dataGaugeBar.Value    = current;
    }

    public void UpdateCompanionHP(float current, float max)
    {
        if (_companionHpBar == null) return;
        _companionHpBar.MaxValue = max;
        _companionHpBar.Value    = current;
    }

    public void UpdateLevel(int level)
    {
        if (_levelLabel != null) _levelLabel.Text = $"LVL {level}";
        // Reset XP bar max for new level
        if (_xpBar != null && _levelSystem != null)
            _xpBar.MaxValue = _levelSystem.XpForNextLevel;
    }

    private void UpdateXP(float xp, float total, float needed)
    {
        if (_xpBar == null) return;
        _xpBar.MaxValue = needed;
        _xpBar.Value    = total;
    }

    private void UpdateSynergyGauge(float current, float max)
    {
        if (_synergyBar == null) return;
        _synergyBar.MaxValue = max;
        _synergyBar.Value    = current;

        // Leuchtet auf wenn bereit
        var synergyLabel = GetNodeOrNull<Label>("SynergyGauge/SynergyLabel");
        if (synergyLabel != null)
        {
            bool ready = current >= max;
            synergyLabel.AddThemeColorOverride("font_color",
                ready ? new Color(1f, 0.6f, 1f) : new Color(0.9f, 0.5f, 1.0f));
            synergyLabel.Text = ready ? "SYNERGY  [Q]" : "SYNERGY";
        }
    }

    private void OnAbsorptionComplete(string skillId, int newLevel)
    {
        ShowAbsorptionPopup(skillId, newLevel);
    }

    public void ShowAbsorptionPopup(string skillId, int level)
    {
        if (_absorptionPopup == null) return;
        _absorptionPopup.Text = $"[ {skillId} ]  Lv{level}  absorbed!";
        _absorptionPopup.Visible = true;
        _popupTimer.Start();
    }

    // Lock-On: zeige ◎ über dem Ziel in Screen-Space
    private Label _lockOnLabel;
    private Camera3D _camera;

    private void UpdateLockOnIndicator()
    {
        // Lazy-init
        if (_lockOnLabel == null)
        {
            _lockOnLabel = new Label();
            _lockOnLabel.Text = "◎";
            _lockOnLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _lockOnLabel.AddThemeColorOverride("font_color", new Color(1f, 0.9f, 0.2f));
            _lockOnLabel.AddThemeFontSizeOverride("font_size", 24);
            _lockOnLabel.Visible = false;
            AddChild(_lockOnLabel);
        }

        if (_camera == null)
            _camera = GetViewport().GetCamera3D();

        // Finde PlayerCamera
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count == 0) { _lockOnLabel.Visible = false; return; }
        var playerNode = players[0] as Node3D;
        var playerCam = playerNode?.GetNodeOrNull<PlayerCamera>("PlayerCamera");
        var target = playerCam?.GetLockOnTarget();

        if (target == null || !IsInstanceValid(target) || _camera == null)
        {
            _lockOnLabel.Visible = false;
            return;
        }

        Vector2 screenPos = _camera.UnprojectPosition(target.GlobalPosition + Vector3.Up * 2f);
        _lockOnLabel.Position = screenPos - new Vector2(12, 12);
        _lockOnLabel.Visible = true;
    }
}
