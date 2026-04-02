using Godot;
using System.Collections.Generic;

/// <summary>
/// Fragment-Menü (Skill-Datenbank UI).
/// CanvasLayer, Layer=9, ProcessMode=Always, startet unsichtbar.
/// Öffnen/Schließen via Tab-Taste (InputAction: fragment_menu).
/// </summary>
public partial class FragmentMenuController : CanvasLayer
{
    private static readonly Dictionary<string, string> SkillDescriptions = new Dictionary<string, string>
    {
        { "common_physical_strength", "Physische Stärke — ATK +2 pro Level" },
        { "common_shadow_veil",       "Shadow Veil — Kurze Unsichtbarkeit nach Dodge" },
        { "legendary_echo_strike",    "Echo Strike — Klonattacke: Doppelinstanz greift mit an" },
        { "common_fire_resistance",   "Feuerresistenz — +5% Feuerresistenz pro Level" },
    };
    private const string DefaultDescription = "Datensignatur-Fragment — Effekt wird analysiert...";

    private bool _isOpen = false;

    // UI-Nodes (programmatisch erstellt)
    private Panel _backgroundPanel;
    private ItemList _skillList;
    private Label _detailName;
    private Label _detailDesc;

    public override void _Ready()
    {
        Layer = 9;
        ProcessMode = ProcessModeEnum.Always;
        BuildUI();
        Hide();
    }

    private void BuildUI()
    {
        // Halbtransparenter Hintergrund
        _backgroundPanel = new Panel();
        _backgroundPanel.SetAnchorsPreset(Control.LayoutPreset.Center);
        _backgroundPanel.CustomMinimumSize = new Vector2(700, 500);
        _backgroundPanel.OffsetLeft = -350f;
        _backgroundPanel.OffsetTop  = -250f;
        _backgroundPanel.OffsetRight = 350f;
        _backgroundPanel.OffsetBottom = 250f;

        var bgStyle = new StyleBoxFlat();
        bgStyle.BgColor = new Color(0.04f, 0.06f, 0.14f, 0.95f);
        bgStyle.BorderColor = new Color(0.2f, 0.8f, 1.0f, 0.85f);
        bgStyle.SetBorderWidthAll(2);
        bgStyle.SetCornerRadiusAll(8);
        bgStyle.ContentMarginLeft   = 16;
        bgStyle.ContentMarginRight  = 16;
        bgStyle.ContentMarginTop    = 12;
        bgStyle.ContentMarginBottom = 12;
        _backgroundPanel.AddThemeStyleboxOverride("panel", bgStyle);
        AddChild(_backgroundPanel);

        // Titel
        var title = new Label();
        title.Text = "FRAGMENT-DATENBANK";
        title.HorizontalAlignment = HorizontalAlignment.Center;
        title.AddThemeColorOverride("font_color", new Color(0.3f, 0.9f, 1.0f));
        title.AddThemeFontSizeOverride("font_size", 18);
        title.SetAnchorsPreset(Control.LayoutPreset.TopWide);
        title.OffsetTop = 12;
        title.OffsetBottom = 40;
        _backgroundPanel.AddChild(title);

        // Haupt-HBox (links Liste, rechts Detail)
        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 16);
        hbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        hbox.OffsetTop    = 48;
        hbox.OffsetBottom = -12;
        hbox.OffsetLeft   = 12;
        hbox.OffsetRight  = -12;
        _backgroundPanel.AddChild(hbox);

        // Linke Liste
        _skillList = new ItemList();
        _skillList.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        _skillList.SizeFlagsVertical   = Control.SizeFlags.ExpandFill;
        _skillList.SelectMode = ItemList.SelectModeEnum.Single;
        _skillList.ItemSelected += OnSkillSelected;
        hbox.AddChild(_skillList);

        // Rechtes Detail-Panel
        var detailPanel = new Panel();
        detailPanel.CustomMinimumSize = new Vector2(300, 0);
        detailPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        var detailStyle = new StyleBoxFlat();
        detailStyle.BgColor = new Color(0.06f, 0.09f, 0.18f, 0.9f);
        detailStyle.BorderColor = new Color(0.15f, 0.6f, 0.9f, 0.7f);
        detailStyle.SetBorderWidthAll(1);
        detailStyle.SetCornerRadiusAll(6);
        detailStyle.ContentMarginLeft   = 14;
        detailStyle.ContentMarginRight  = 14;
        detailStyle.ContentMarginTop    = 14;
        detailStyle.ContentMarginBottom = 14;
        detailPanel.AddThemeStyleboxOverride("panel", detailStyle);
        hbox.AddChild(detailPanel);

        var detailVbox = new VBoxContainer();
        detailVbox.AddThemeConstantOverride("separation", 10);
        detailVbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        detailVbox.OffsetLeft   = 14;
        detailVbox.OffsetRight  = -14;
        detailVbox.OffsetTop    = 14;
        detailVbox.OffsetBottom = -14;
        detailPanel.AddChild(detailVbox);

        _detailName = new Label();
        _detailName.Text = "—";
        _detailName.AutowrapMode = TextServer.AutowrapMode.Word;
        _detailName.AddThemeColorOverride("font_color", new Color(0.9f, 1.0f, 0.6f));
        _detailName.AddThemeFontSizeOverride("font_size", 15);
        detailVbox.AddChild(_detailName);

        _detailDesc = new Label();
        _detailDesc.Text = "";
        _detailDesc.AutowrapMode = TextServer.AutowrapMode.Word;
        _detailDesc.AddThemeColorOverride("font_color", new Color(0.85f, 0.85f, 0.85f));
        _detailDesc.AddThemeFontSizeOverride("font_size", 13);
        _detailDesc.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        detailVbox.AddChild(_detailDesc);

        // Hinweis unten
        var hint = new Label();
        hint.Text = "[Tab] oder [ESC] Schließen";
        hint.HorizontalAlignment = HorizontalAlignment.Right;
        hint.AddThemeColorOverride("font_color", new Color(0.5f, 0.5f, 0.5f));
        hint.AddThemeFontSizeOverride("font_size", 11);
        detailVbox.AddChild(hint);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (InputMap.HasAction("fragment_menu") && Input.IsActionJustPressed("fragment_menu"))
            {
                ToggleMenu();
                GetViewport().SetInputAsHandled();
                return;
            }

            if (_isOpen && key.PhysicalKeycode == Key.Escape)
            {
                // ESC nur schließen wenn PauseMenu nicht offen ist
                bool pauseOpen = GameManager.Instance?.CurrentState == GameManager.GameState.Paused;
                if (!pauseOpen)
                {
                    CloseMenu();
                    GetViewport().SetInputAsHandled();
                }
            }
        }
    }

    private void ToggleMenu()
    {
        if (_isOpen) CloseMenu();
        else OpenMenu();
    }

    private void OpenMenu()
    {
        _isOpen = true;
        RefreshSkillList();
        Show();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void CloseMenu()
    {
        _isOpen = false;
        Hide();
        // Maus nur zurückstellen wenn Spiel nicht pausiert
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Paused)
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void RefreshSkillList()
    {
        _skillList.Clear();
        _detailName.Text = "—";
        _detailDesc.Text = "";

        // FragmentSystem suchen
        var fs = GetTree().Root.FindChild("FragmentSystem", true, false) as FragmentSystem;
        if (fs == null)
        {
            // Auch direkt am Player suchen
            var players = GetTree().GetNodesInGroup("player");
            if (players.Count > 0)
                fs = (players[0] as Node3D)?.GetNodeOrNull<FragmentSystem>("FragmentSystem");
        }

        if (fs == null || fs.SkillLevels.Count == 0)
        {
            _skillList.AddItem("(Keine Fragmente absorbiert)");
            return;
        }

        foreach (var kvp in fs.SkillLevels)
        {
            string displayName = $"{kvp.Key}  [Lv{kvp.Value}]";
            _skillList.AddItem(displayName);
            // Metadaten für spätere Auswahl
            int idx = _skillList.ItemCount - 1;
            _skillList.SetItemMetadata(idx, kvp.Key);
        }
    }

    private void OnSkillSelected(long index)
    {
        string skillId = _skillList.GetItemMetadata((int)index).AsString();
        _detailName.Text = skillId;

        if (SkillDescriptions.TryGetValue(skillId, out string desc))
            _detailDesc.Text = desc;
        else
            _detailDesc.Text = DefaultDescription;
    }
}
