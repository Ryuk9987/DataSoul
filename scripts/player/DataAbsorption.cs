using Godot;

public partial class DataAbsorption : Node
{
    [Signal] public delegate void AbsorptionStartedEventHandler(string skillId);
    [Signal] public delegate void AbsorptionCompleteEventHandler(string skillId, int newLevel);

    private static readonly string PARTICLE_SCENE_PATH = "res://scenes/vfx/DataFragmentParticles.tscn";
    private static PackedScene _particleScene;

    private FragmentSystem _fragmentSystem;
    private Timer _absorptionTimer;
    private string _pendingSkillId = "";
    private bool _isAbsorbing = false;
    private Vector3 _absorptionPosition = Vector3.Zero;

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

        // Partikel-Szene vorab laden
        if (_particleScene == null)
            _particleScene = ResourceLoader.Load<PackedScene>(PARTICLE_SCENE_PATH);
    }

    public void StartAbsorption(EnemyBase enemy)
    {
        if (_isAbsorbing) return;
        _pendingSkillId = enemy.SkillDropId;
        _isAbsorbing = true;
        _absorptionPosition = enemy.GlobalPosition;

        EmitSignal(SignalName.AbsorptionStarted, _pendingSkillId);
        AudioManager.Instance?.PlaySfx("absorption", GetParent<Node3D>()?.GlobalPosition ?? Vector3.Zero);

        // Datenfragment-Partikel an der Position des getöteten Gegners spawnen
        SpawnDataFragmentParticles(_absorptionPosition);

        _absorptionTimer.Start();
    }

    private void SpawnDataFragmentParticles(Vector3 position)
    {
        if (_particleScene == null)
        {
            GD.PrintErr("[DataAbsorption] DataFragmentParticles.tscn nicht geladen!");
            return;
        }

        var particles = _particleScene.Instantiate<GpuParticles3D>();
        if (particles == null) return;

        // In die aktuelle Szene einfügen und positionieren
        GetTree().CurrentScene.AddChild(particles);
        particles.GlobalPosition = position + Vector3.Up * 0.5f;
        particles.Emitting = true;

        // Auto-Destroy: Lifetime (0.8s) + kleine Reserve
        var destroyTimer = new Timer();
        destroyTimer.WaitTime = particles.Lifetime + 0.3f;
        destroyTimer.OneShot = true;
        destroyTimer.Timeout += () =>
        {
            if (IsInstanceValid(particles)) particles.QueueFree();
            destroyTimer.QueueFree();
        };
        GetTree().CurrentScene.AddChild(destroyTimer);
        destroyTimer.Start();
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
