using Godot;
using System;

public partial class CharacterCreationController : Control
{
    public override void _Ready()
    {
        // Character creation logic here
    }

    private void OnSaveButtonPressed()
    {
        // Save character data
        GetTree().ChangeSceneToFile("res://scenes/ui/IntroSequence.tscn");
    }
}