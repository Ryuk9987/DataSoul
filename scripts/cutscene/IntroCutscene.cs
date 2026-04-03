using Godot;
using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// IntroCutscene — steuert die vollständige Intro-Sequenz im Beschwörungsraum.
///
/// Ablauf:
///   SummoningTriggered → ritual_cutscene → lyra_first_contact
///   → lyra_first_contact_{background} → first_quest_briefing → leaving_akademie
///   → GetTree().ChangeSceneToFile("Aldenmere.tscn")
///
/// Glitch-Events (6+Idle):
///   #1 ritual[2]   0.4 / 0.3s
///   #2 ritual[4]   0.3 / 0.2s
///   #3 ritual[7]   0.6 / 0.8s
///   #4 ritual[10]  0.2 / 0.3s
///   #5 briefing[1] 0.1 / 1 frame
///   #6 briefing[8] 0.15 / 0.2s
///   Idle           0.03–0.05 (dauerhaft ab ritual[8])
///
/// Kamera-Bewegungen via Tween auf CutsceneCamera (Camera3D).
/// Spieler-Input wird während Cutscene gesperrt.
/// NPCs reagieren NIE auf Glitch-Events (.hack-Prinzip).
/// </summary>
public partial class IntroCutscene : Node
{
    // ── Nodes (werden in _Ready() gesucht) ────────────────────────────────────
    private Camera3D _cutsceneCamera;
    private Camera3D _playerCamera;
    private CanvasLayer _glitchOverlay;
    private ShaderMaterial _glitchMat;
    private Node3D _particles;           // Parent-Node der 5 GpuParticles3D-Emitter
    private Node3D _summoningFocus;      // Portal-Mesh/Node
    private OmniLight3D _runeGlow;       // RuneGlow Licht für glow_intensity-Simulation
    private AudioStreamPlayer _ambientMusic;

    // ── Dialogue-Daten ────────────────────────────────────────────────────────
    private Dictionary<string, List<DialogueLine>> _dialogues = new();

    private struct DialogueLine
    {
        public string Speaker;
        public string Text;
        public string Emotion;
    }

    // ── Zustand ───────────────────────────────────────────────────────────────
    private bool _isRunning = false;

    // Dialog-Cursor: welche Sequenz, welcher Index gerade angezeigt wird
    private List<DialogueLine> _currentSeq;
    private int _currentLineIdx = 0;
    private string _currentSeqName = "";

    // Dialog-Timer (auto-advance nach N Sekunden oder F-Taste)
    private Timer _lineTimer;
    private bool _waitingForInput = false;
    private float _lineDuration = 5.0f;

    // Speichert den Idle-Glitch-Intensitätswert (aktiviert ab ritual[8])
    private float _idleGlitchIntensity = 0f;
    private bool _idleGlitchActive = false;

    // Tween-Ref für Kamera
    private Tween _cameraTween;

    // ── Kamera-Positionen ─────────────────────────────────────────────────────
    // Beschwoerungsraum-Koordinaten (approx., angepasst an Szene)
    private static readonly Vector3 CAM_BIRD_EYE     = new(0f, 8f,  0f);     // Vogelperspektive
    private static readonly Vector3 CAM_ALDRIC        = new(-3f, 1.8f, 2f);   // Magister Aldric Halbtotale
    private static readonly Vector3 CAM_RUNE_ZOOM     = new(0f, 2f, 2f);      // Zoom auf Runenkreis
    private static readonly Vector3 CAM_RUNE_ORBIT_A  = new(3f, 3f, 3f);      // Orbit A
    private static readonly Vector3 CAM_WIDE_ROOM     = new(0f, 4f, 7f);      // Weiter Raum-Schuss
    private static readonly Vector3 CAM_FROG_EYE      = new(0f, 0.3f, 3f);    // Bodenperspektive
    private static readonly Vector3 CAM_THIRD_PERSON  = new(0f, 2.5f, 5f);    // Third-Person Standard
    private static readonly Vector3 CAM_OTS_LYRA      = new(-1.5f, 1.8f, 3f); // Über-Schulter hinter Lyra
    private static readonly Vector3 CAM_LYRA_CLOSE    = new(-2f, 1.6f, 1.5f); // Close-Up Lyra
    private static readonly Vector3 CAM_LYRA_FRONT    = new(0f, 1.7f, 2.5f);  // Frontansicht Lyra
    private static readonly Vector3 CAM_ALDRIC_WIDE   = new(0f, 3f, 8f);      // Weite Briefing-Einst.
    private static readonly Vector3 CAM_ALDRIC_CLOSE  = new(2f, 1.7f, 1.5f);  // Close-Up Aldric

    // ── LookAt-Targets ────────────────────────────────────────────────────────
    private static readonly Vector3 TARGET_RUNE    = new(0f, 0f,  0f);
    private static readonly Vector3 TARGET_ALDRIC  = new(-3f, 1.6f, 0f);
    private static readonly Vector3 TARGET_LYRA    = new(-2f, 1.5f, 0f);
    private static readonly Vector3 TARGET_PLAYER  = new(0f, 1.2f, 0f);
    private static readonly Vector3 TARGET_ROOM    = new(0f, 2f,  0f);

    // ─────────────────────────────────────────────────────────────────────────
    public override void _Ready()
    {
        LoadDialogues();
        BuildGlitchOverlay();
        SetupLineTimer();
        BuildCutsceneCamera();

        // Cutscene startet automatisch beim Laden der Szene (kommt immer von CharacterCreation)
        // Kurzes Defer damit alle Nodes vollständig initialisiert sind
        CallDeferred(MethodName.OnSummoningTriggered);
    }

    // ── Öffentliche API ───────────────────────────────────────────────────────

    /// <summary>Wird von BeschwoerungsraumZone via Signal SummoningTriggered aufgerufen.</summary>
    public void OnSummoningTriggered()
    {
        if (_isRunning) return;
        GD.Print("[IntroCutscene] Cutscene gestartet.");
        _isRunning = true;

        // Szene-Refs einsammeln
        CollectSceneRefs();

        // Spieler-Input sperren
        SetPlayerInput(false);

        // Spieler-Kamera deaktivieren
        if (_playerCamera != null) _playerCamera.Current = false;
        if (_cutsceneCamera != null) _cutsceneCamera.Current = true;

        // Kamera: Vogelperspektive auf Runenkreis (Startposition)
        SetCameraImmediate(CAM_BIRD_EYE, TARGET_RUNE);

        // Sequenz starten
        StartSequence("ritual_cutscene");
    }

    // ── Dialoge laden ─────────────────────────────────────────────────────────

    private void LoadDialogues()
    {
        const string path = "res://data/dialogues/intro_sequence.json";
        if (!FileAccess.FileExists(path))
        {
            GD.PrintErr($"[IntroCutscene] intro_sequence.json nicht gefunden: {path}");
            return;
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("dialogues");

            foreach (var seq in root.EnumerateObject())
            {
                var lines = new List<DialogueLine>();
                foreach (var entry in seq.Value.EnumerateArray())
                {
                    lines.Add(new DialogueLine
                    {
                        Speaker = entry.TryGetProperty("speaker", out var sp)  ? sp.GetString()  : "?",
                        Text    = entry.TryGetProperty("text",    out var tx)  ? tx.GetString()  : "",
                        Emotion = entry.TryGetProperty("emotion", out var em)  ? em.GetString()  : ""
                    });
                }
                _dialogues[seq.Name] = lines;
                GD.Print($"[IntroCutscene] Sequenz '{seq.Name}' geladen: {lines.Count} Zeilen.");
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[IntroCutscene] JSON-Parse-Fehler: {ex.Message}");
        }
    }

    // ── Szene-Refs ────────────────────────────────────────────────────────────

    private void CollectSceneRefs()
    {
        var scene = GetTree().CurrentScene;

        // Partikel
        _particles     = scene.GetNodeOrNull<Node3D>("Particles");
        _summoningFocus = scene.GetNodeOrNull<Node3D>("SummoningFocus");
        _ambientMusic  = scene.GetNodeOrNull<AudioStreamPlayer>("AmbientMusic");

        // Runenglühen
        _runeGlow      = scene.GetNodeOrNull<OmniLight3D>("Lighting/RuneGlow");

        // Spieler-Kamera
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count > 0 && players[0] is Node3D player)
        {
            _playerCamera = player.GetNodeOrNull<Camera3D>("PlayerCamera/SpringArm3D/Camera3D");
            if (_playerCamera == null)
                _playerCamera = player.FindChild("Camera3D", true, false) as Camera3D;
        }

        GD.Print($"[IntroCutscene] Refs: Particles={_particles != null}, Focus={_summoningFocus != null}, " +
                 $"Ambient={_ambientMusic != null}, PlayerCam={_playerCamera != null}");
    }

    // ── Sequenz-Steuerung ────────────────────────────────────────────────────

    private void StartSequence(string seqName)
    {
        if (!_dialogues.TryGetValue(seqName, out var lines))
        {
            GD.PrintErr($"[IntroCutscene] Sequenz nicht gefunden: {seqName}");
            OnSequenceComplete(seqName);
            return;
        }

        _currentSeqName = seqName;
        _currentSeq     = lines;
        _currentLineIdx = 0;

        GD.Print($"[IntroCutscene] Starte Sequenz: {seqName}");
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (_currentSeq == null || _currentLineIdx >= _currentSeq.Count)
        {
            OnSequenceComplete(_currentSeqName);
            return;
        }

        var line = _currentSeq[_currentLineIdx];

        // Kamera + Glitch vor Dialog-Anzeige
        HandleDirectionBefore(_currentSeqName, _currentLineIdx);

        // Dialogue anzeigen
        float duration = ComputeDuration(line.Text);
        DialogueSystem.Instance?.ShowLine(line.Speaker, line.Text, duration);

        // Kamera + Glitch nach Dialog-Anzeige
        HandleDirectionAfter(_currentSeqName, _currentLineIdx);

        // Timer für Auto-Advance
        _waitingForInput = true;
        _lineTimer.WaitTime = duration + 0.5f; // Etwas Puffer
        _lineTimer.Start();
    }

    private float ComputeDuration(string text)
    {
        // ~15 Zeichen/Sekunde Lesegeschwindigkeit, min 3s, max 9s
        return Mathf.Clamp(text.Length / 15f, 3f, 9f);
    }

    private void OnLineTimerTimeout()
    {
        if (!_waitingForInput) return;
        _waitingForInput = false;
        _currentLineIdx++;
        ShowCurrentLine();
    }

    private void OnSequenceComplete(string seqName)
    {
        GD.Print($"[IntroCutscene] Sequenz '{seqName}' abgeschlossen.");

        switch (seqName)
        {
            case "ritual_cutscene":
                // Kamera zu Third-Person, Lyra betritt
                MoveCameraTo(CAM_THIRD_PERSON, TARGET_PLAYER, 1.5f);
                StartSequence("lyra_first_contact");
                break;

            case "lyra_first_contact":
                // Background-spezifische Variante auswählen
                string bgKey = GetBackgroundDialogueKey();
                StartSequence(bgKey);
                break;

            case string s when s.StartsWith("lyra_first_contact_"):
                // Kamera zu Aldric-Briefing-Setup
                MoveCameraTo(CAM_ALDRIC_WIDE, TARGET_ROOM, 1.2f);
                StartSequence("first_quest_briefing");
                break;

            case "first_quest_briefing":
                // Kurzer schwarzer Schnitt (0.3s) vor leaving_akademie
                var tween = CreateTween();
                tween.TweenCallback(Callable.From(() =>
                {
                    // Glitch-Overlay kurz auf schwarz (Schnitt-Simulation)
                    if (_glitchMat != null)
                        _glitchMat.SetShaderParameter("glitch_intensity", 0f);
                }));
                tween.TweenInterval(0.3f);
                tween.TweenCallback(Callable.From(() => StartSequence("leaving_akademie")));
                break;

            case "leaving_akademie":
                // Cutscene endet → Gameplay
                EndCutscene();
                break;

            default:
                GD.PrintErr($"[IntroCutscene] Unbekannte Sequenz nach Ende: {seqName}");
                EndCutscene();
                break;
        }
    }

    // ── Regie-Anweisungen ─────────────────────────────────────────────────────
    // Kamera + Glitch-Events pro Dialog-Index
    // Referenz: intro_sequence_regie.md

    private void HandleDirectionBefore(string seq, int idx)
    {
        switch (seq)
        {
            case "ritual_cutscene":
                HandleRitualBefore(idx);
                break;
            case "lyra_first_contact":
                HandleLyraBefore(idx);
                break;
            case "first_quest_briefing":
                HandleBriefingBefore(idx);
                break;
            case "leaving_akademie":
                HandleLeavingBefore(idx);
                break;
        }
    }

    private void HandleDirectionAfter(string seq, int idx)
    {
        // Für Nachher-Effekte (z.B. Partikel-Aktivierung)
        switch (seq)
        {
            case "ritual_cutscene":
                HandleRitualAfter(idx);
                break;
        }
    }

    // ritual_cutscene ─────────────────────────────────────────────────────────

    private void HandleRitualBefore(int idx)
    {
        switch (idx)
        {
            case 0: // Zeremonienmeister spricht ersten Satz
                MoveCameraTo(CAM_ALDRIC, TARGET_ALDRIC, 2.0f);
                break;

            case 1: // Beschwörungsspruch — Partikel aktivieren, Rune leuchten
                MoveCameraTo(CAM_RUNE_ZOOM, TARGET_RUNE, 3.0f);
                ActivateParticles(true, 1, 2);
                TweenRuneGlow(1.8f, 6.0f, 3f);
                break;

            case 2: // NARRATOR: Runenkreis erwacht — Glitch #1, Orbit
                TriggerScreenGlitch(0.4f, 0.3f);
                MoveCameraOrbit(CAM_RUNE_ORBIT_A, TARGET_RUNE, 3.0f);
                ActivateParticles(true, 1, 2, 3, 4, 5);
                break;

            case 3: // Portal öffnet sich
                MoveCameraTo(CAM_WIDE_ROOM, TARGET_ROOM, 1.5f);
                ScalePortal(Vector3.One, 1.5f);
                break;

            case 4: // Akademiewächter — Glitch #2
                TriggerScreenGlitch(0.3f, 0.2f);
                break;

            case 5: // Zeremonienmeister: "Still."
                // Kamera hält, Ambient ducken (via AudioStreamPlayer)
                DuckAmbient(0.5f, 0.3f);
                break;

            case 6: // NARRATOR: Portal reißt auf
                MoveCameraTo(CAM_FROG_EYE, TARGET_PLAYER, 0.5f);
                break;

            case 7: // NARRATOR: Flimmern, Rauschen — Glitch #3
                TriggerScreenGlitch(0.6f, 0.8f);
                FlickerTorch(0.5f);
                DeactivateAllParticles();
                break;

            case 8: // NARRATOR: Du öffnest die Augen — Idle-Glitch aktivieren
                MoveCameraTo(CAM_THIRD_PERSON, TARGET_PLAYER, 1.0f);
                _idleGlitchIntensity = 0.05f;
                _idleGlitchActive = true;
                SetIdleGlitch(0.05f);
                break;

            case 9: // Zeremonienmeister: "Es hat funktioniert."
                MoveCameraTo(CAM_THIRD_PERSON, TARGET_PLAYER, 1.2f);
                break;

            case 10: // NARRATOR: letzte Zeile — Glitch #4, Rune verblasst
                TriggerScreenGlitch(0.2f, 0.3f);
                TweenRuneGlow(0.6f, 4.0f, 4f); // Verblassen
                break;
        }
    }

    private void HandleRitualAfter(int idx)
    {
        // Nach dem letzten Ritual-Satz: Übergang-Vorbereitung
        if (idx == 10)
        {
            // Glow weiter verblassen lassen (bereits via Tween eingeleitet)
        }
    }

    // lyra_first_contact ──────────────────────────────────────────────────────

    private void HandleLyraBefore(int idx)
    {
        switch (idx)
        {
            case 0: // Lyra betritt
                MoveCameraTo(CAM_LYRA_FRONT, TARGET_LYRA, 1.5f);
                break;
            case 1: // Über-die-Schulter
                MoveCameraTo(CAM_OTS_LYRA, TARGET_PLAYER, 1.0f);
                break;
            case 2: // Über-die-Schulter halten
                break;
            case 3: // Frontansicht Lyra
                MoveCameraTo(CAM_LYRA_FRONT, TARGET_LYRA, 0.8f);
                break;
            case 4: // Reverse-Einstellung (zeigt Protagonisten)
                MoveCameraTo(new Vector3(0f, 1.8f, -3f), TARGET_PLAYER, 0.6f);
                break;
            case 5: // Close-Up Lyra
                MoveCameraTo(CAM_LYRA_CLOSE, TARGET_LYRA, 0.5f);
                break;
            case 6: // Zurück zur Halbtotale
                MoveCameraTo(CAM_THIRD_PERSON, TARGET_ROOM, 1.0f);
                break;
        }
    }

    // first_quest_briefing ────────────────────────────────────────────────────

    private void HandleBriefingBefore(int idx)
    {
        switch (idx)
        {
            case 0: // Weite Einstellung — alle 3 Charaktere
                MoveCameraTo(CAM_ALDRIC_WIDE, TARGET_ALDRIC, 1.5f);
                break;
            case 1: // Glitch #5 (Aldrics Robe — 1 Frame)
                TriggerScreenGlitch(0.1f, 0.05f);
                break;
            case 2: // Langsam heranfahren
                MoveCameraTo(new Vector3(1f, 2.5f, 5f), TARGET_ALDRIC, 6f);
                DuckAmbient(0.7f, 0.3f);
                break;
            case 3: // Hände Close-Up → Halbtotale
                MoveCameraTo(new Vector3(0f, 1.0f, 2f), new Vector3(0f, 0.8f, 0f), 0.5f);
                var tween = CreateTween();
                tween.TweenInterval(1.5f);
                tween.TweenCallback(Callable.From(() =>
                    MoveCameraTo(CAM_ALDRIC_WIDE, TARGET_ALDRIC, 0.8f)));
                break;
            case 4: // Schwenk zu Lyra
                MoveCameraTo(CAM_OTS_LYRA, TARGET_PLAYER, 0.8f);
                break;
            case 5: // Über-Schulter hinter Lyra, Aldric füllt Bild
                MoveCameraTo(new Vector3(-1.2f, 1.8f, 2f), TARGET_ALDRIC, 0.6f);
                break;
            case 6: // Close-Up Lyra
                MoveCameraTo(CAM_LYRA_CLOSE, TARGET_LYRA, 0.5f);
                break;
            case 7: // Zurück zur weiten Einstellung
                MoveCameraTo(CAM_ALDRIC_WIDE, TARGET_ROOM, 1.0f);
                break;
            case 8: // Langsam Close-Up Aldric — Glitch #6
                MoveCameraTo(CAM_ALDRIC_CLOSE, TARGET_ALDRIC, 4f);
                TriggerScreenGlitch(0.15f, 0.2f);
                break;
            case 9: // Cut Lyra → Protagonist → Lyra
                MoveCameraTo(CAM_LYRA_FRONT, TARGET_LYRA, 0.4f);
                break;
            case 10: // Aldric dreht sich ab — weite Einstellung
                MoveCameraTo(CAM_ALDRIC_WIDE, TARGET_ROOM, 0.8f);
                break;
        }
    }

    // leaving_akademie ────────────────────────────────────────────────────────

    private void HandleLeavingBefore(int idx)
    {
        switch (idx)
        {
            case 0: // Kamera: Begleitung von hinten
                MoveCameraTo(new Vector3(0f, 2f, 6f), new Vector3(0f, 1.5f, 0f), 1.2f);
                break;
            case 1: // Profilaufnahme
                MoveCameraTo(new Vector3(4f, 1.8f, 0f), new Vector3(0f, 1.5f, 0f), 1.0f);
                SetIdleGlitch(0.03f); // Randflimmern
                break;
            case 2: // Zoom-Out → Aldenmere Stadtkulisse
                MoveCameraTo(new Vector3(0f, 4f, 12f), new Vector3(0f, 1.5f, 0f), 3.0f);
                SetIdleGlitch(0.03f);
                break;
        }
    }

    // ── Kamera-Helfer ─────────────────────────────────────────────────────────

    private void BuildCutsceneCamera()
    {
        // Cutscene-Kamera als Child dieses Nodes erstellen
        _cutsceneCamera = new Camera3D();
        _cutsceneCamera.Name = "CutsceneCamera";
        _cutsceneCamera.Fov = 65f;
        _cutsceneCamera.Current = false;
        AddChild(_cutsceneCamera);
        SetCameraImmediate(CAM_BIRD_EYE, TARGET_RUNE);
    }

    private void SetCameraImmediate(Vector3 pos, Vector3 lookAt)
    {
        if (_cutsceneCamera == null) return;
        _cutsceneCamera.GlobalPosition = pos;
        var dir = (lookAt - pos).Normalized();
        if (dir.LengthSquared() > 0.001f)
        {
            // Fallback: wenn dir fast parallel zu Up ist (z.B. Bird-Eye), Back als Up verwenden
            var up = (Mathf.Abs(dir.Dot(Vector3.Up)) > 0.99f) ? Vector3.Back : Vector3.Up;
            _cutsceneCamera.GlobalBasis = Basis.LookingAt(dir, up);
        }
    }

    private void MoveCameraTo(Vector3 pos, Vector3 lookAt, float duration)
    {
        if (_cutsceneCamera == null) return;

        _cameraTween?.Kill();
        _cameraTween = CreateTween();
        _cameraTween.SetTrans(Tween.TransitionType.Sine);
        _cameraTween.SetEase(Tween.EaseType.InOut);

        var startPos = _cutsceneCamera.GlobalPosition;
        var endPos   = pos;

        _cameraTween.TweenMethod(
            Callable.From((float t) =>
            {
                if (_cutsceneCamera == null || !IsInstanceValid(_cutsceneCamera)) return;
                var curPos = startPos.Lerp(endPos, t);
                _cutsceneCamera.GlobalPosition = curPos;
                var dir = (lookAt - curPos).Normalized();
                if (dir.LengthSquared() > 0.001f)
                    _cutsceneCamera.GlobalBasis = Basis.LookingAt(dir, Vector3.Up);
            }),
            0f, 1f, duration
        );
    }

    private void MoveCameraOrbit(Vector3 endPos, Vector3 pivotLookAt, float duration)
    {
        // Orbit: kreist um den Pivot-Punkt (pivotLookAt) auf einem Kreisbogen
        if (_cutsceneCamera == null) return;

        _cameraTween?.Kill();
        _cameraTween = CreateTween();
        _cameraTween.SetTrans(Tween.TransitionType.Sine);
        _cameraTween.SetEase(Tween.EaseType.InOut);

        var startPos = _cutsceneCamera.GlobalPosition;

        _cameraTween.TweenMethod(
            Callable.From((float t) =>
            {
                if (_cutsceneCamera == null || !IsInstanceValid(_cutsceneCamera)) return;
                var curPos = startPos.Lerp(endPos, t);
                _cutsceneCamera.GlobalPosition = curPos;
                var dir = (pivotLookAt - curPos).Normalized();
                if (dir.LengthSquared() > 0.001f)
                    _cutsceneCamera.GlobalBasis = Basis.LookingAt(dir, Vector3.Up);
            }),
            0f, 1f, duration
        );
    }

    // ── Glitch-Overlay ────────────────────────────────────────────────────────

    private void BuildGlitchOverlay()
    {
        // CanvasLayer mit ColorRect + ScreenGlitch-Shader
        _glitchOverlay = new CanvasLayer();
        _glitchOverlay.Name = "CutsceneGlitchOverlay";
        _glitchOverlay.Layer = 10; // Über alles, inkl. DialogueSystem
        AddChild(_glitchOverlay);

        var rect = new ColorRect();
        rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        rect.Color = new Color(0, 0, 0, 0); // Transparent — Shader zeichnet selbst

        var shader = ResourceLoader.Load<Shader>("res://assets/shaders/ScreenGlitch.gdshader");
        if (shader != null)
        {
            _glitchMat = new ShaderMaterial();
            _glitchMat.Shader = shader;
            _glitchMat.SetShaderParameter("glitch_intensity", 0f);
            _glitchMat.SetShaderParameter("glitch_speed", 5f);
            _glitchMat.SetShaderParameter("aberration_strength", 0.004f);
            rect.Material = _glitchMat;
        }
        else
        {
            GD.PrintErr("[IntroCutscene] ScreenGlitch.gdshader nicht gefunden!");
        }

        _glitchOverlay.AddChild(rect);
    }

    /// <summary>Kurzer Screen-Glitch-Burst (Intensität, Dauer in Sekunden).</summary>
    private void TriggerScreenGlitch(float intensity, float duration)
    {
        if (_glitchMat == null) return;

        var tween = CreateTween();
        tween.TweenCallback(Callable.From(() =>
            _glitchMat.SetShaderParameter("glitch_intensity", intensity)));
        tween.TweenInterval(duration);
        tween.TweenCallback(Callable.From(() =>
            _glitchMat.SetShaderParameter("glitch_intensity", _idleGlitchActive ? _idleGlitchIntensity : 0f)));
    }

    /// <summary>Setzt dauerhaften Idle-Glitch (niedrige Intensität).</summary>
    private void SetIdleGlitch(float intensity)
    {
        _idleGlitchIntensity = intensity;
        _idleGlitchActive = true;
        _glitchMat?.SetShaderParameter("glitch_intensity", intensity);
    }

    // ── Partikel ──────────────────────────────────────────────────────────────

    private void ActivateParticles(bool emitting, params int[] indices)
    {
        if (_particles == null) return;
        foreach (int i in indices)
        {
            var emitter = _particles.GetNodeOrNull<GpuParticles3D>($"Emitter{i}");
            if (emitter != null) emitter.Emitting = emitting;
        }
    }

    private void DeactivateAllParticles()
    {
        if (_particles == null) return;
        for (int i = 1; i <= 5; i++)
        {
            var emitter = _particles.GetNodeOrNull<GpuParticles3D>($"Emitter{i}");
            if (emitter != null) emitter.Emitting = false;
        }
    }

    // ── Portal-Skalierung ────────────────────────────────────────────────────

    private void ScalePortal(Vector3 targetScale, float duration)
    {
        if (_summoningFocus == null) return;
        _summoningFocus.Scale = Vector3.Zero;

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Elastic);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_summoningFocus, "scale", targetScale, duration);
    }

    // ── Rune-Glow ─────────────────────────────────────────────────────────────

    private void TweenRuneGlow(float targetEnergy, float targetRange, float duration)
    {
        if (_runeGlow == null) return;
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(_runeGlow, "light_energy", targetEnergy, duration);
        tween.Parallel().TweenProperty(_runeGlow, "omni_range", targetRange, duration);
    }

    // ── Ambient-Audio ────────────────────────────────────────────────────────

    private void DuckAmbient(float targetVolumeLinear, float duration)
    {
        if (_ambientMusic == null) return;
        // VolumeDb-Tween: -12 dB Normal, ducken auf ca. -20 dB
        float targetDb = Mathf.LinearToDb(targetVolumeLinear) * 10f - 12f;
        var tween = CreateTween();
        tween.TweenProperty(_ambientMusic, "volume_db", targetDb, duration);
        // Nach 1s zurückfahren
        tween.TweenInterval(1f);
        tween.TweenProperty(_ambientMusic, "volume_db", -12f, duration);
    }

    // ── Fackel-Flackern ──────────────────────────────────────────────────────

    private void FlickerTorch(float duration)
    {
        if (_runeGlow == null) return;
        // Schnelle Intensitäts-Oszillation
        var tween = CreateTween();
        tween.SetLoops(Mathf.RoundToInt(duration / 0.05f));
        tween.TweenProperty(_runeGlow, "light_energy", 0.3f, 0.025f);
        tween.TweenProperty(_runeGlow, "light_energy", 2.0f, 0.025f);
        // Danach normalisieren
        var resetTween = CreateTween();
        resetTween.TweenInterval(duration + 0.1f);
        resetTween.TweenProperty(_runeGlow, "light_energy", 1.8f, 0.3f);
    }

    // ── Spieler-Input ────────────────────────────────────────────────────────

    private void SetPlayerInput(bool enabled)
    {
        var players = GetTree().GetNodesInGroup("player");
        foreach (var p in players)
        {
            if (p is Node node)
            {
                // PlayerController (CharacterBody3D) ProcessMode setzen
                node.ProcessMode = enabled
                    ? Node.ProcessModeEnum.Inherit
                    : Node.ProcessModeEnum.Disabled;
            }
        }

        // Maus: während Cutscene sichtbar (kein Captured)
        Input.MouseMode = enabled
            ? Input.MouseModeEnum.Captured
            : Input.MouseModeEnum.Visible;
    }

    // ── Background-Dialog Mapping ─────────────────────────────────────────────

    private string GetBackgroundDialogueKey()
    {
        // GameSession.PlayerBackground (PlayerStats.Background enum)
        string suffix = GameSession.PlayerBackground switch
        {
            PlayerStats.Background.Programmer => "programmer",
            PlayerStats.Background.Gamer      => "gamer",
            PlayerStats.Background.Hacker     => "hacker",
            PlayerStats.Background.Creator    => "creator",
            PlayerStats.Background.Analyst    => "analyst",
            _                                 => "programmer" // Fallback
        };
        string key = $"lyra_first_contact_{suffix}";
        // Prüfen ob vorhanden, sonst Fallback
        if (!_dialogues.ContainsKey(key))
        {
            GD.PrintErr($"[IntroCutscene] Background-Sequenz nicht gefunden: {key} → Fallback: programmer");
            key = "lyra_first_contact_programmer";
        }
        return key;
    }

    // ── Timer-Setup ──────────────────────────────────────────────────────────

    private void SetupLineTimer()
    {
        _lineTimer = new Timer { OneShot = true };
        _lineTimer.Timeout += OnLineTimerTimeout;
        AddChild(_lineTimer);
    }

    // ── F-Taste zum Weiterschalten ───────────────────────────────────────────

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isRunning || !_waitingForInput) return;
        if (@event is InputEventKey key && key.Pressed && !key.Echo
            && key.PhysicalKeycode == Key.F)
        {
            // Vorzeitig weiterschalten
            _lineTimer.Stop();
            _waitingForInput = false;
            _currentLineIdx++;
            ShowCurrentLine();
        }
    }

    // ── Cutscene Ende ────────────────────────────────────────────────────────

    private void EndCutscene()
    {
        GD.Print("[IntroCutscene] Cutscene beendet — Gameplay beginnt.");

        // Idle-Glitch bleibt auf 0.03 (dauerhafter subtiler Effekt)
        SetIdleGlitch(0.03f);

        // Spieler-Input wieder aktivieren
        SetPlayerInput(true);

        // Spieler-Kamera wieder aktiv
        if (_playerCamera != null) _playerCamera.Current = true;
        if (_cutsceneCamera != null) _cutsceneCamera.Current = false;

        // Dialogue ausblenden
        DialogueSystem.Instance?.HideImmediate();

        // HUD einblenden (falls HUDController existiert)
        var hud = GetTree().Root.FindChild("PlayerHUD", true, false) as CanvasLayer;
        hud?.Show();

        _isRunning = false;

        // Szene wechseln zu Aldenmere.tscn
        var tween = CreateTween();
        tween.TweenInterval(1.0f);
        tween.TweenCallback(Callable.From(() =>
            GetTree().ChangeSceneToFile("res://scenes/world/aldenmere/Aldenmere.tscn")));
    }
}
