using Godot;

public partial class IntroSequenceController : Control
{
    private Label _sceneLabel;
    private Label _panelText;
    private Label _progressLabel;

    private int _currentPanel = 0;

    // (SceneLabel, PanelText)
    private readonly (string scene, string text)[] _panels = new[]
    {
        (
            "[Dunkelheit. Ferne Stimmen. Ein Ritual.]",
            "Das Beschwörungsritual der Akademie von Valdris ist älter als die Aufzeichnungen.\nMan sagt, es ruft einen Helden aus einer anderen Realität — jemanden mit einer Datensignatur die sich von allem Bekannten unterscheidet."
        ),
        (
            "[Ein grelles Licht. Datenmuster flackern. Der Boden löst sich auf.]",
            "Du spürst wie deine Realität auseinanderbricht. Nicht schmerzhaft — eher wie ein Programm das neu startet.\nAlles was du kanntest: ein Cache der geleert wird."
        ),
        (
            "[Stille. Dann: Stimmen. Code. Hexagonale Muster.]",
            "DataWorld empfängt dich nicht sanft. Deine Datensignatur ist fragmentiert — der Übergang war nicht sauber.\nEtwas an dir ist... anders. Als hättest du Root-Access wo andere keinen haben."
        ),
        (
            "[Du öffnest die Augen. Steinruinen. Jemand steht vor dir.]",
            "\"Laut Protokoll sollte ich zuerst fragen ob du verletzt bist. Also... bist du verletzt?\nIch meine — du bist gerade durch einen Datenstrom aus einer anderen Realität gefallen, also ich nehme an... ja?\"\n\n— Lyra, Heiladeptin der Akademie"
        ),
        (
            "[Die Firewall Ruins. Dein erster Tag in DataWorld.]",
            "Ihr Name ist Lyra. Sie soll dich begleiten — offiziell.\nInoffiziell: sie wurde zugeteilt weil sie 'noch nicht bereit für wichtigere Missionen' ist.\nDas kränkt sie. Du siehst es in ihren Augen.\n\n[F / Leertaste — Ins Abenteuer!]"
        ),
    };

    public override void _Ready()
    {
        _sceneLabel    = GetNode<Label>("CenterContainer/VBox/SceneLabel");
        _panelText     = GetNode<Label>("CenterContainer/VBox/PanelText");
        _progressLabel = GetNode<Label>("CenterContainer/VBox/ProgressLabel");
        ShowPanel(0);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode == Key.F || key.Keycode == Key.Space)
            {
                _currentPanel++;
                if (_currentPanel < _panels.Length)
                    ShowPanel(_currentPanel);
                else
                    GetTree().ChangeSceneToFile("res://scenes/world/FirewallRuins/FirewallRuins.tscn");
            }
        }
    }

    private void ShowPanel(int index)
    {
        _sceneLabel.Text    = _panels[index].scene;
        _panelText.Text     = _panels[index].text;
        _progressLabel.Text = $"{index + 1} / {_panels.Length}";
    }
}
