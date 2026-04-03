using Godot;

/// <summary>
/// CutsceneTTS — TTS Voice Acting für Cutscene-Dialoge.
/// Nutzt Godots eingebautes TTS (System-Stimmen: SAPI unter Windows, espeak/festival unter Linux).
/// Stimm-Profile (Pitch + Rate) je nach Sprecher-Name.
/// </summary>
public static class CutsceneTTS
{
    public static void Speak(string speaker, string text)
    {
        // Vorherige Rede stoppen
        DisplayServer.TtsStop();

        var voices = DisplayServer.TtsGetVoices();
        if (voices.Count == 0)
        {
            GD.PrintErr("[CutsceneTTS] Keine TTS-Stimmen verfügbar.");
            return;
        }

        string voiceId = GetVoiceForSpeaker(speaker, voices);
        float pitch    = GetPitchForSpeaker(speaker);
        float rate     = GetRateForSpeaker(speaker);

        GD.Print($"[CutsceneTTS] {speaker}: \"{text}\" (pitch={pitch}, rate={rate})");
        DisplayServer.TtsSpeak(text, voiceId, 100, pitch, rate);
    }

    public static void Stop() => DisplayServer.TtsStop();

    // ── Stimm-Auswahl ─────────────────────────────────────────────────────────

    private static string GetVoiceForSpeaker(
        string speaker,
        Godot.Collections.Array<Godot.Collections.Dictionary> voices)
    {
        if (voices.Count == 0) return "";

        // Versuche, eine zur Sprache passende Stimme zu finden (Deutsch bevorzugt).
        // Fallback: erste verfügbare Stimme (plattformübergreifend sicher).
        foreach (var voice in voices)
        {
            string lang = voice.ContainsKey("language") ? voice["language"].AsString() : "";
            if (lang.StartsWith("de"))
                return voice["id"].AsString();
        }

        // Falls keine deutsche Stimme: je nach Sprecher evtl. zweite/dritte nehmen
        int voiceIndex = speaker switch
        {
            "Zeremonienmeister" => 0,
            "Magister Aldric"   => 0,
            "Lyra"              => voices.Count > 1 ? 1 : 0,
            _                   => 0
        };

        return voices[voiceIndex]["id"].AsString();
    }

    // ── Pitch-Profile ─────────────────────────────────────────────────────────

    private static float GetPitchForSpeaker(string speaker) => speaker switch
    {
        "Zeremonienmeister" => 0.7f,   // Tief, feierlich
        "Magister Aldric"   => 0.75f,  // Autoritär
        "Lyra"              => 1.2f,   // Hoch, jung
        "NARRATOR"          => 1.0f,   // Neutral
        _                   => 1.0f
    };

    // ── Rate-Profile ──────────────────────────────────────────────────────────

    private static float GetRateForSpeaker(string speaker) => speaker switch
    {
        "Zeremonienmeister" => 0.85f,  // Langsam, feierlich
        "Magister Aldric"   => 0.9f,
        "Lyra"              => 1.1f,   // Etwas schneller
        "NARRATOR"          => 0.95f,
        _                   => 1.0f
    };
}
