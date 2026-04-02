using Godot;
using System.Collections.Generic;

/// <summary>
/// Zentraler Audio-Manager für alle SFX und Ambient-Musik.
/// Als Node in FirewallRuins.tscn oder als Autoload nutzbar.
/// </summary>
public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    private readonly Dictionary<string, AudioStream> _streams = new();
    private readonly List<AudioStreamPlayer3D> _pool = new();
    private AudioStreamPlayer _musicPlayer;
    private const int POOL_SIZE = 8;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;

        // 3D-SFX Pool
        for (int i = 0; i < POOL_SIZE; i++)
        {
            var p = new AudioStreamPlayer3D();
            p.MaxDistance = 30f;
            p.UnitSize = 5f;
            AddChild(p);
            _pool.Add(p);
        }

        // 2D Music Player
        _musicPlayer = new AudioStreamPlayer();
        _musicPlayer.VolumeDb = -12f;
        AddChild(_musicPlayer);

        PreloadAll();

        // Ambient nach kurzem Delay starten
        var timer = new Timer { WaitTime = 0.5f, OneShot = true };
        timer.Timeout += () => PlayAmbient("ambient_ruins");
        AddChild(timer);
        timer.Start();
    }

    private void PreloadAll()
    {
        var sfxNames = new[]
        {
            "hit_light", "hit_heavy", "dodge", "dodge_counter",
            "skill_activate", "absorption", "player_hurt", "player_death"
        };

        foreach (var name in sfxNames)
        {
            var path = $"res://assets/audio/sfx/{name}.wav";
            var stream = ResourceLoader.Load<AudioStream>(path);
            if (stream != null)
                _streams[name] = stream;
            else
                GD.PrintErr($"[AudioManager] SFX nicht gefunden: {path}");
        }

        // Ambient
        var ambient = ResourceLoader.Load<AudioStream>("res://assets/audio/music/ambient_ruins.wav");
        if (ambient != null)
            _streams["ambient_ruins"] = ambient;
    }

    /// <summary>Spielt einen 3D-Sound an einer Position ab.</summary>
    public void PlaySfx(string name, Vector3 position, float volumeDb = 0f)
    {
        if (!_streams.TryGetValue(name, out var stream)) return;

        // Freien Player aus dem Pool holen
        AudioStreamPlayer3D player = null;
        foreach (var p in _pool)
        {
            if (!p.Playing) { player = p; break; }
        }
        if (player == null) player = _pool[0]; // Fallback: ältesten überschreiben

        player.Stream = stream;
        player.VolumeDb = volumeDb;
        player.GlobalPosition = position;
        player.Play();
    }

    /// <summary>Spielt einen 2D-Sound (UI, global) ab.</summary>
    public void PlaySfx2D(string name, float volumeDb = 0f)
    {
        if (!_streams.TryGetValue(name, out var stream)) return;
        var player = new AudioStreamPlayer();
        player.Stream = stream;
        player.VolumeDb = volumeDb;
        player.Autoplay = false;
        AddChild(player);
        player.Play();
        // Auto-cleanup
        player.Finished += player.QueueFree;
    }

    public void PlayAmbient(string name, bool loop = true)
    {
        if (!_streams.TryGetValue(name, out var stream)) return;
        if (stream is AudioStreamWav wav)
            wav.LoopMode = loop ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled;
        _musicPlayer.Stream = stream;
        _musicPlayer.Play();
    }

    public void StopAmbient() => _musicPlayer.Stop();
}
