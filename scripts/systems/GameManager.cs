using Godot;

public partial class GameManager : Node
{
    public enum GameState { Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    public static GameManager Instance { get; private set; }

    private PlayerStats _playerStats;
    private LevelSystem _levelSystem;
    private HUDController _hud;
    private CanvasLayer _pauseMenu;

    public override void _Ready()
    {
        Instance = this;
        // Muss auch im pausierten Zustand Input verarbeiten
        ProcessMode = ProcessModeEnum.Always;
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

        if (_levelSystem != null && _hud != null)
            _levelSystem.LevelUp += (level) => _hud.UpdateLevel(level);

        GetTree().Connect("node_added", new Callable(this, MethodName.OnNodeAdded));

        // PauseMenu suchen (wird als Child in FirewallRuins.tscn instanziiert)
        _pauseMenu = GetTree().Root.FindChild("PauseMenu", true, false) as CanvasLayer;
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

        var timer = new Timer();
        timer.WaitTime = 3.0f;
        timer.OneShot = true;
        timer.Timeout += () => GetTree().ReloadCurrentScene();
        AddChild(timer);
        timer.Start();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _pauseMenu?.Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            // DataNode-Menü hat Vorrang — dessen _UnhandledInput setzt Input als handled
            if (DataNode.IsMenuOpen) return;

            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Paused;
                GetTree().Paused = true;
                Input.MouseMode = Input.MouseModeEnum.Visible;

                // PauseMenu beim ersten ESC suchen falls noch nicht gefunden
                if (_pauseMenu == null)
                    _pauseMenu = GetTree().Root.FindChild("PauseMenu", true, false) as CanvasLayer;

                _pauseMenu?.Show();
            }
            else if (CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }
}
