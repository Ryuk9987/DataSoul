using Godot;
using System.Collections.Generic;

public partial class DialogueSystem : CanvasLayer
{
    public static DialogueSystem Instance { get; private set; }

    public struct DialogueLine
    {
        public string Speaker;
        public string Text;
        public float Duration;
        public DialogueLine(string speaker, string text, float duration = 4f)
        { Speaker = speaker; Text = text; Duration = duration; }
    }

    private Panel _panel;
    private Label _speakerLabel;
    private Label _textLabel;
    private Label _continueHint;
    private Timer _autoHideTimer;

    private Queue<DialogueLine> _queue = new Queue<DialogueLine>();
    private bool _showing = false;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        Layer = 8;

        // Build UI programmatically so no tscn needed
        _panel = new Panel();
        _panel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        _panel.CustomMinimumSize = new Vector2(0, 170);
        _panel.OffsetLeft   = 40f;
        _panel.OffsetRight  = -40f;
        _panel.OffsetTop    = -185f;
        _panel.OffsetBottom = -10f;

        // Dunkles Halbransparentes Panel mit Cyan-Border
        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = new Color(0.04f, 0.06f, 0.12f, 0.92f);
        styleBox.BorderColor = new Color(0.2f, 0.7f, 1.0f, 0.8f);
        styleBox.SetBorderWidthAll(2);
        styleBox.SetCornerRadiusAll(6);
        styleBox.ContentMarginLeft = 16;
        styleBox.ContentMarginRight = 16;
        styleBox.ContentMarginTop = 10;
        styleBox.ContentMarginBottom = 10;
        _panel.AddThemeStyleboxOverride("panel", styleBox);
        AddChild(_panel);

        var vbox = new VBoxContainer();
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        vbox.AddThemeConstantOverride("separation", 6);
        vbox.OffsetLeft = 16; vbox.OffsetTop = 10; vbox.OffsetRight = -16; vbox.OffsetBottom = -10;
        _panel.AddChild(vbox);

        _speakerLabel = new Label();
        _speakerLabel.AddThemeColorOverride("font_color", GameColors.DataCyan);
        _speakerLabel.AddThemeFontSizeOverride("font_size", 13);
        vbox.AddChild(_speakerLabel);

        _textLabel = new Label();
        _textLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        _textLabel.AddThemeColorOverride("font_color", new Color(0.95f, 0.95f, 0.95f));
        _textLabel.AddThemeFontSizeOverride("font_size", 15);
        _textLabel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        vbox.AddChild(_textLabel);

        _continueHint = new Label();
        _continueHint.Text = "[F] Weiter";
        _continueHint.HorizontalAlignment = HorizontalAlignment.Right;
        _continueHint.AddThemeColorOverride("font_color", new Color(0.6f, 0.6f, 0.6f));
        _continueHint.AddThemeFontSizeOverride("font_size", 11);
        vbox.AddChild(_continueHint);

        _autoHideTimer = new Timer { OneShot = true };
        _autoHideTimer.Timeout += OnAutoHide;
        AddChild(_autoHideTimer);

        _panel.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_showing && @event is InputEventKey key && key.Pressed
            && key.PhysicalKeycode == Key.F)
        {
            _autoHideTimer.Stop();
            OnAutoHide();
        }
    }

    public void ShowLine(string speaker, string text, float duration = 4f)
        => ShowSequence(new[] { new DialogueLine(speaker, text, duration) });

    public void ShowSequence(DialogueLine[] lines)
    {
        foreach (var line in lines) _queue.Enqueue(line);
        if (!_showing) ShowNext();
    }

    private void ShowNext()
    {
        if (_queue.Count == 0) { _panel.Visible = false; _showing = false; return; }

        _showing = true;
        var line = _queue.Dequeue();
        _speakerLabel.Text = line.Speaker;
        _textLabel.Text = line.Text;
        _panel.Visible = true;
        float waitTime = Mathf.Max(line.Duration, 0.1f);
        if (waitTime <= 0f) { OnAutoHide(); return; }
        _autoHideTimer.WaitTime = waitTime;
        _autoHideTimer.Start();
    }

    private void OnAutoHide()
    {
        if (_queue.Count > 0) ShowNext();
        else { _panel.Visible = false; _showing = false; }
    }

    public void HideImmediate()
    {
        _autoHideTimer.Stop();
        _queue.Clear();
        _panel.Visible = false;
        _showing = false;
    }
}
