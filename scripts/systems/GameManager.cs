using Godot;

public partial class GameManager : Node
{
    public enum GameState { Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    public static GameManager Instance { get; private set; }

    private PlayerStats _playerStats;
    private LevelSystem _levelSystem;
    private HUDController _hud;

    public override void _Ready()
    {
        Instance = this;
        CallDeferred(MethodName.ConnectSystems);
    }

    private void ConnectSystems()
    {
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count > 0)
        {
            var player = players[0];
            _playerStats = (player as Node3D)?.GetNodeOrNull<PlayerStats>("PlayerStats");
            _levelSystem = (player as Node3D)?.GetNodeOrNull<LevelSystem>("LevelSystem");

            if (_playerStats != null)
                _playerStats.Died += OnPlayerDied;
        }

        _hud = GetTree().Root.FindChild("PlayerHUD", true, false) as HUDController;

        // Connect level-up to HUD
        if (_levelSystem != null && _hud != null)
        {
            _levelSystem.LevelUp += (level) => _hud.UpdateLevel(level);
        }

        // Connect enemy kills to XP
        GetTree().Connect("node_added", new Callable(this, MethodName.OnNodeAdded));
    }

    private void OnNodeAdded(Node node)
    {
        if (node is EnemyBase enemy)
            enemy.Died += OnEnemyDied;
    }

    private void OnEnemyDied(EnemyBase enemy)
    {
        if (_levelSystem == null) return;
        string type = enemy is BossBase ? "boss" : "normal";
        float xp = LevelSystem.GetXpForEnemy(type, _levelSystem.Level);
        _levelSystem.AddXp(xp);
    }

    private void OnPlayerDied()
    {
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        GD.Print("[GameManager] Game Over!");

        // Simple restart after 3 seconds
        var timer = new Timer();
        timer.WaitTime = 3.0f;
        timer.OneShot = true;
        timer.Timeout += () => GetTree().ReloadCurrentScene();
        AddChild(timer);
        timer.Start();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Paused;
                GetTree().Paused = true;
            }
            else if (CurrentState == GameState.Paused)
            {
                CurrentState = GameState.Playing;
                GetTree().Paused = false;
            }
        }
    }
}
