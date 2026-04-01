using Godot;

public partial class PlayerCamera : Node3D
{
    [Export] public float DefaultDistance = 5.0f;
    [Export] public float MinZoom = 2.0f;
    [Export] public float MaxZoom = 10.0f;
    [Export] public float MouseSensitivity = 0.003f;
    [Export] public float CombatShiftHeight = 0.3f;

    private SpringArm3D _springArm;
    private Camera3D _camera;
    private float _yaw = 0f;
    private float _pitch = -15.0f;
    private float _currentDistance;
    private bool _inCombat = false;
    private Node3D _lockOnTarget = null;
    private Node3D _followTarget = null;

    public Vector3 ForwardDirection => (-new Vector3(Mathf.Sin(_yaw), 0, Mathf.Cos(_yaw))).Normalized();
    public Vector3 RightDirection => new Vector3(Mathf.Cos(_yaw), 0, -Mathf.Sin(_yaw)).Normalized();

    public override void _Ready()
    {
        _currentDistance = DefaultDistance;
        _springArm = GetNodeOrNull<SpringArm3D>("SpringArm3D");
        if (_springArm != null)
        {
            _camera = _springArm.GetNodeOrNull<Camera3D>("Camera3D");
            _springArm.SpringLength = _currentDistance;
        }

        // Decouple from parent rotation — follow position only
        TopLevel = true;
        _followTarget = GetParent() as Node3D;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && _lockOnTarget == null)
        {
            _yaw -= mouseMotion.Relative.X * MouseSensitivity;
            _pitch = Mathf.Clamp(_pitch - mouseMotion.Relative.Y * MouseSensitivity, -1.0f, 0.35f);
        }

        if (@event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.ButtonIndex == MouseButton.WheelUp)
                _currentDistance = Mathf.Clamp(_currentDistance - 0.5f, MinZoom, MaxZoom);
            else if (mouseBtn.ButtonIndex == MouseButton.WheelDown)
                _currentDistance = Mathf.Clamp(_currentDistance + 0.5f, MinZoom, MaxZoom);
            if (_springArm != null) _springArm.SpringLength = _currentDistance;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
                Input.MouseMode = Input.MouseModeEnum.Visible;
            if (keyEvent.PhysicalKeycode == Key.F)
                Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public override void _Process(double delta)
    {
        // Follow player position
        if (_followTarget != null)
        {
            float targetHeight = 1.5f + (_inCombat ? CombatShiftHeight : 0f);
            GlobalPosition = GlobalPosition.Lerp(
                _followTarget.GlobalPosition + Vector3.Up * targetHeight,
                (float)delta * 20f);
        }

        // Apply yaw + pitch via rotation
        Rotation = new Vector3(0, _yaw, 0);
        if (_springArm != null)
            _springArm.Rotation = new Vector3(_pitch, 0, 0);

        // Lock-on: override yaw toward target
        if (_lockOnTarget != null && IsInstanceValid(_lockOnTarget))
        {
            var dir = (_lockOnTarget.GlobalPosition - GlobalPosition);
            dir.Y = 0;
            if (dir.LengthSquared() > 0.001f)
                _yaw = Mathf.LerpAngle(_yaw, Mathf.Atan2(-dir.X, -dir.Z), (float)delta * 10f);
        }
    }

    public void SetCombatMode(bool active) => _inCombat = active;

    public void SetLockOnTarget(Node3D target) => _lockOnTarget = target;
    public void ClearLockOn() => _lockOnTarget = null;
    public Node3D GetLockOnTarget() => _lockOnTarget;
}
