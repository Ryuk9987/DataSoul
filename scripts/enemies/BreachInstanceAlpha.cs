using Godot;
using System.Collections.Generic;

public partial class BreachInstanceAlpha : BossBase
{
    [Signal] public delegate void AoEWarningEventHandler();
    [Signal] public delegate void AoEDamageEventHandler(Vector3 origin, float radius);

    [Export] public float CloneSpawnInterval = 15.0f;
    [Export] public float AoEInterval = 8.0f;
    [Export] public float AoERadius = 6.0f;

    private List<Node3D> _activeClones = new List<Node3D>();
    private float _cloneTimer = 0f;
    private float _aoETimer = 0f;
    private float _aoEWarningTimer = 0f;
    private bool _aoEWarningActive = false;

    // Phase 2: one "real" clone among fakes
    private int _realCloneIndex = 0;

    public override void _Ready()
    {
        MaxHp = 500f;
        Atk = 18f;
        Def = 6f;
        Spd = 5f;
        SkillDropId = "legendary_echo_strike";
        PhaseThresholds = new float[] { 0.6f, 0.3f };
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead) return;
        base._PhysicsProcess(delta);

        _cloneTimer -= (float)delta;
        _aoETimer -= (float)delta;

        switch (CurrentPhase)
        {
            case 1:
                // Spawn 2 temporary clones every 15s
                if (_cloneTimer <= 0)
                {
                    SpawnTemporaryClones(2, 5.0f);
                    _cloneTimer = CloneSpawnInterval;
                }
                break;

            case 2:
                // 3 permanent clones, one is real
                if (_activeClones.Count < 3)
                    SpawnPermanentClones(3);
                break;

            case 3:
                // AoE wave every 8s with 2s warning
                if (_aoEWarningActive)
                {
                    _aoEWarningTimer -= (float)delta;
                    if (_aoEWarningTimer <= 0)
                    {
                        _aoEWarningActive = false;
                        TriggerAoE();
                        _aoETimer = AoEInterval;
                    }
                }
                else if (_aoETimer <= 0)
                {
                    EmitSignal(SignalName.AoEWarning);
                    _aoEWarningActive = true;
                    _aoEWarningTimer = 2.0f;
                }
                break;
        }
    }

    protected override void OnPhaseChange(int newPhase)
    {
        switch (newPhase)
        {
            case 2:
                // Remove temp clones
                ClearClones();
                _cloneTimer = 0f;
                break;
            case 3:
                // Clones merge back, speed+atk boost
                ClearClones();
                Spd = 11f;
                Atk = 23f;
                _aoETimer = 3f; // First AoE after 3s
                break;
        }
    }

    private void SpawnTemporaryClones(int count, float lifetime)
    {
        for (int i = 0; i < count; i++)
        {
            var clone = CreateClone(false);
            _activeClones.Add(clone);

            // Auto-remove after lifetime
            var timer = new Timer();
            timer.WaitTime = lifetime;
            timer.OneShot = true;
            int idx = _activeClones.Count - 1;
            timer.Timeout += () => {
                if (IsInstanceValid(clone)) { clone.QueueFree(); _activeClones.Remove(clone); }
                timer.QueueFree();
            };
            AddChild(timer);
            timer.Start();
        }
    }

    private void SpawnPermanentClones(int count)
    {
        ClearClones();
        _realCloneIndex = GD.RandRange(0, count - 1);
        for (int i = 0; i < count; i++)
        {
            bool isReal = (i == _realCloneIndex);
            var clone = CreateClone(isReal);
            _activeClones.Add(clone);
        }
    }

    private Node3D CreateClone(bool isReal)
    {
        // Create a simple visual clone (MeshInstance3D placeholder)
        var clone = new MeshInstance3D();
        var capsule = new CapsuleMesh();
        clone.Mesh = capsule;

        // Real clone is brighter
        var mat = new StandardMaterial3D();
        mat.AlbedoColor = isReal
            ? new Color(1f, 0.3f, 0.3f, 0.9f)
            : new Color(0.5f, 0.1f, 0.1f, 0.5f);
        mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        clone.MaterialOverride = mat;

        // Add to scene FIRST, then set GlobalPosition (requires scene tree)
        GetParent().AddChild(clone);

        var rng = new RandomNumberGenerator();
        rng.Randomize();
        clone.GlobalPosition = GlobalPosition + new Vector3(
            rng.RandfRange(-4f, 4f), 0, rng.RandfRange(-4f, 4f));

        return clone;
    }

    private void ClearClones()
    {
        foreach (var clone in _activeClones)
            if (IsInstanceValid(clone)) clone.QueueFree();
        _activeClones.Clear();
    }

    private void TriggerAoE()
    {
        EmitSignal(SignalName.AoEDamage, GlobalPosition, AoERadius);

        // Damage player if in range
        var player = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;
        if (player != null)
        {
            float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
            if (dist <= AoERadius)
            {
                var controller = player as PlayerController;
                if (controller == null || !controller.IsInvincible)
                {
                    var stats = player.GetNodeOrNull<PlayerStats>("PlayerStats");
                    stats?.TakeDamage(Atk * 1.5f);
                }
            }
        }
    }

    protected override void Die()
    {
        ClearClones();
        base.Die();
    }
}
