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
    private float _pitch = -15.0f;
    private float _currentDistance;
    private bool _inCombat = false;
    private Node3D _lockOnTarget = null;

    public Vector3 ForwardDirection => -GlobalTransform.Basis.Z.Normalized();
    public Vector3 RightDirection => GlobalTransform.Basis.X.Normalized();

    public override void _Ready()
    {
        _currentDistance = DefaultDistance;
        _springArm = GetNodeOrNull<SpringArm3D>("SpringArm3D");
        if (_springArm != null)
        {
            _camera = _springArm.GetNodeOrNull<Camera3D>("Camera3D");
            _springArm.SpringLength = _currentDistance;
        }
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && _lockOnTarget == null)
        {
            RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            _pitch = Mathf.Clamp(_pitch - mouseMotion.Relative.Y * MouseSensitivity * 57.3f, -60f, 20f);
            if (_springArm != null)
                _springArm.RotationDegrees = new Vector3(_pitch, 0, 0);
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
        // Combat shift: raise camera slightly in combat
        if (_springArm != null)
        {
            float targetHeight = _inCombat ? CombatShiftHeight : 0f;
            var pos = _springArm.Position;
            pos.Y = Mathf.Lerp(pos.Y, targetHeight, (float)delta * 5f);
            _springArm.Position = pos;
        }

        // Lock-on: rotate toward target
        if (_lockOnTarget != null && IsInstanceValid(_lockOnTarget))
        {
            var dir = (_lockOnTarget.GlobalPosition - GlobalPosition).Normalized();
            dir.Y = 0;
            if (dir.LengthSquared() > 0.001f)
            {
                var targetBasis = Basis.LookingAt(dir, Vector3.Up);
                GlobalTransform = new Transform3D(
                    GlobalTransform.Basis.Slerp(targetBasis, (float)delta * 10f),
                    GlobalPosition);
            }
        }
    }

    public void SetCombatMode(bool active) => _inCombat = active;

    public void SetLockOnTarget(Node3D target) => _lockOnTarget = target;
    public void ClearLockOn() => _lockOnTarget = null;
    public Node3D GetLockOnTarget() => _lockOnTarget;
}
