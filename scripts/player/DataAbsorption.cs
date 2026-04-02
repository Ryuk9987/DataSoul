using Godot;

public partial class DataAbsorption : Node
{
    [Signal] public delegate void AbsorptionStartedEventHandler(string skillId);
    [Signal] public delegate void AbsorptionCompleteEventHandler(string skillId, int newLevel);

    private FragmentSystem _fragmentSystem;
    private Timer _absorptionTimer;
    private string _pendingSkillId = "";
    private bool _isAbsorbing = false;

    public bool IsAbsorbing => _isAbsorbing;

    public override void _Ready()
    {
        _fragmentSystem = GetParent().GetNodeOrNull<FragmentSystem>("FragmentSystem");
        // Also try to find FragmentSystem from GameManager
        if (_fragmentSystem == null)
            _fragmentSystem = GetTree().Root.FindChild("FragmentSystem", true, false) as FragmentSystem;

        _absorptionTimer = new Timer();
        _absorptionTimer.WaitTime = 1.5f;
        _absorptionTimer.OneShot = true;
        _absorptionTimer.Timeout += OnAbsorptionComplete;
        AddChild(_absorptionTimer);
    }

    public void StartAbsorption(EnemyBase enemy)
    {
        if (_isAbsorbing) return;
        _pendingSkillId = enemy.SkillDropId;
        _isAbsorbing = true;
        EmitSignal(SignalName.AbsorptionStarted, _pendingSkillId);
        AudioManager.Instance?.PlaySfx("absorption", GetParent<Node3D>()?.GlobalPosition ?? Vector3.Zero);
        _absorptionTimer.Start();
    }

    private void OnAbsorptionComplete()
    {
        _isAbsorbing = false;
        if (_pendingSkillId != "" && _fragmentSystem != null)
        {
            _fragmentSystem.AbsorbSkill(_pendingSkillId);
            int newLevel = _fragmentSystem.GetSkillLevel(_pendingSkillId);
            EmitSignal(SignalName.AbsorptionComplete, _pendingSkillId, newLevel);
        }
        _pendingSkillId = "";
    }
}
