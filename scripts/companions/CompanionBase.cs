using Godot;

public abstract partial class CompanionBase : CharacterBody3D
{
    [Export] public float FollowDistanceMin = 4.0f;
    [Export] public float FollowDistanceMax = 6.0f;
    [Export] public float RetreatDistance = 2.0f; // Retreat if enemy closer than this
    [Export] public float AiTickInterval = 0.5f;
    [Export] public float MoveSpeed = 6.0f;

    protected CharacterBody3D FollowTarget;
    protected NavigationAgent3D NavAgent;

    private float _aiTickTimer = 0f;

    public override void _Ready()
    {
        NavAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        FollowTarget = GetTree().Root.FindChild("Player", true, false) as CharacterBody3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (FollowTarget == null) return;

        // AI tick
        _aiTickTimer -= (float)delta;
        if (_aiTickTimer <= 0)
        {
            _aiTickTimer = AiTickInterval;
            ExecuteAI();
        }

        // Movement
        DoFollow(delta);
    }

    private void DoFollow(double delta)
    {
        if (FollowTarget == null) return;

        float dist = GlobalPosition.DistanceTo(FollowTarget.GlobalPosition);

        // Check for nearby enemies — retreat if too close
        bool enemyTooClose = false;
        var enemies = GetTree().GetNodesInGroup("enemies");
        foreach (Node3D enemy in enemies)
        {
            if (GlobalPosition.DistanceTo(enemy.GlobalPosition) < RetreatDistance)
            {
                enemyTooClose = true;
                break;
            }
        }

        Vector3 targetPos;
        if (enemyTooClose)
        {
            // Move away from enemies, toward player
            targetPos = FollowTarget.GlobalPosition;
        }
        else if (dist > FollowDistanceMax)
        {
            targetPos = FollowTarget.GlobalPosition;
        }
        else if (dist < FollowDistanceMin)
        {
            // Back off a bit
            var awayDir = (GlobalPosition - FollowTarget.GlobalPosition).Normalized();
            targetPos = GlobalPosition + awayDir * 0.5f;
        }
        else
        {
            // In range — don't move
            Velocity = new Vector3(0, Velocity.Y - 20f * (float)delta, 0);
            MoveAndSlide();
            return;
        }

        if (NavAgent != null)
        {
            NavAgent.TargetPosition = targetPos;
            var nextPos = NavAgent.GetNextPathPosition();
            var dir = (nextPos - GlobalPosition).Normalized();
            dir.Y = 0;
            Velocity = new Vector3(dir.X * MoveSpeed, Velocity.Y - 20f * (float)delta, dir.Z * MoveSpeed);
        }
        else
        {
            var dir = (targetPos - GlobalPosition).Normalized();
            dir.Y = 0;
            Velocity = new Vector3(dir.X * MoveSpeed, Velocity.Y - 20f * (float)delta, dir.Z * MoveSpeed);
        }
        MoveAndSlide();
    }

    protected abstract void ExecuteAI();
}
