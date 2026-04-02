using Godot;
using System;

public partial class IntroSequenceController : Control
{
    private int currentPanel = 0;
    private readonly string[] panels = new string[]
    {
        "[Dunkelheit. Ferne Stimmen. Ein Ritual.]",
        "Das Beschwörungsritual der Akademie von Valdris ist älter als die Aufzeichnungen. Man sagt, es ruft einen Helden aus einer anderen Realität — jemanden mit einer Datensignatur die sich von allem Bekannten unterscheidet.",
        "[Ein grelles Licht. Datenmuster flackern. Der Boden löst sich auf.]",
        "Du spürst wie deine Realität auseinanderbricht. Nicht schmerzhaft — eher wie ein Programm das neu startet. Alles was du kanntest: ein Cache der geleert wird.",
        "[Stille. Dann: Stimmen. Code. Hexagonale Muster.]",
        "DataWorld empfängt dich nicht sanft. Deine Datensignatur ist fragmentiert — der Übergang war nicht sauber. Etwas an dir ist... anders. Als hättest du Root-Access wo andere keinen haben.",
        "[Du öffnest die Augen. Steinruinen. Jemand steht vor dir.]",
        "*Laut Protokoll sollte ich zuerst fragen ob du verletzt bist. Also... bist du verletzt? Ich meine — du bist gerade durch einen Datenstrom aus einer anderen Realität gefallen, also ich nehme an... ja?"
        "— Lyra, Heiladeptin der Akademie",
        "[Die Firewall Ruins. Dein erster Tag in DataWorld.]",
        "Ihr Name ist Lyra. Sie soll dich begleiten — offiziell. Inoffiziell: sie wurde zugeteilt weil sie 'noch nicht bereit für wichtigere Missionen' ist. Das kränkt sie. Du siehst es in ihren Augen.",
        "[Drücke F um zu beginnen]"
    };

    public override void _Ready()
    {
        GetNode<Label>("PanelText").Text = panels[currentPanel];
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.F || eventKey.Keycode == Key.Space)
            {
                currentPanel++;
                if (currentPanel < panels.Length)
                {
                    GetNode<Label>("PanelText").Text = panels[currentPanel];
                }
                else
                {
                    GetTree().ChangeSceneToFile("res://scenes/world/FirewallRuins/FirewallRuins.tscn");
                }
            }
        }
    }
}