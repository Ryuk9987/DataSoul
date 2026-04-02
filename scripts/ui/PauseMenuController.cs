using Godot;

public partial class PauseMenuController : CanvasLayer
{
    public override void _Ready()
    {
        // Muss auch im pausierten Zustand laufen
        ProcessMode = ProcessModeEnum.Always;
        Hide();

        var resumeBtn = GetNodeOrNull<Button>("Panel/VBox/ResumeButton");
        var quitBtn = GetNodeOrNull<Button>("Panel/VBox/QuitButton");

        if (resumeBtn != null) resumeBtn.Pressed += OnResume;
        if (quitBtn != null) quitBtn.Pressed += OnQuit;
    }

    private void OnResume()
    {
        var gm = GetTree().Root.FindChild("GameManager", true, false) as GameManager;
        gm?.ResumeGame();
    }

    private void OnQuit()
    {
        GetTree().Paused = false;
        GetTree().Quit();
    }
}
