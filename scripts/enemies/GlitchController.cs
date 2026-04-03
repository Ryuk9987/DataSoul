using Godot;

/// <summary>
/// Steuert den GlitchShader auf einem Gegner-Mesh.
/// Wird als Node zu jedem Gegner hinzugefügt.
/// - Im Normalzustand: glitch_intensity 0.15–0.3
/// - Unter 25% HP: overload_mode = true
/// - Boss (BreachInstanceAlpha): Intensität pro Phase (0.2 / 0.5 / 0.8)
/// </summary>
public partial class GlitchController : Node
{
    [Export] public float NormalIntensity { get; set; } = 0.2f;
    [Export] public bool IsBoss { get; set; } = false;

    private ShaderMaterial _shaderMat;
    private EnemyBase _enemy;
    private BossBase _boss;
    private int _lastPhase = 1;

    // Boss phase intensities
    private static readonly float[] BossPhaseIntensity = { 0.2f, 0.5f, 0.8f };

    public override void _Ready()
    {
        _enemy = GetParent() as EnemyBase;
        _boss = GetParent() as BossBase;

        // Finde das erste MeshInstance3D im Parent (direkt oder im Baum)
        var mesh = FindMesh(GetParent());
        if (mesh == null)
        {
            GD.PrintErr($"[GlitchController] Kein MeshInstance3D gefunden in {GetParent().Name}");
            return;
        }

        // Shader-Material erstellen und zuweisen
        var shader = ResourceLoader.Load<Shader>("res://assets/shaders/GlitchShader.gdshader");
        if (shader == null)
        {
            GD.PrintErr("[GlitchController] GlitchShader.gdshader nicht gefunden!");
            return;
        }

        _shaderMat = new ShaderMaterial();
        _shaderMat.Shader = shader;
        _shaderMat.SetShaderParameter("glitch_intensity", NormalIntensity);
        _shaderMat.SetShaderParameter("overload_mode", false);
        _shaderMat.SetShaderParameter("glitch_speed", 2.0f);
        _shaderMat.SetShaderParameter("emission_strength", 0.5f);

        // Albedo-Farbe von bestehendem Material übernehmen falls vorhanden
        var existing = mesh.GetActiveMaterial(0) as StandardMaterial3D;
        if (existing != null)
            _shaderMat.SetShaderParameter("albedo_color", existing.AlbedoColor);

        mesh.MaterialOverride = _shaderMat;
    }

    public override void _Process(double delta)
    {
        if (_shaderMat == null || _enemy == null || _enemy.IsDead) return;

        float hpPercent = _enemy.GetHpPercent();
        bool isOverload = hpPercent <= 0.25f;
        _shaderMat.SetShaderParameter("overload_mode", isOverload);

        if (IsBoss && _boss != null)
        {
            int phase = _boss.CurrentPhase;
            if (phase != _lastPhase)
            {
                _lastPhase = phase;
                int idx = Mathf.Clamp(phase - 1, 0, BossPhaseIntensity.Length - 1);
                _shaderMat.SetShaderParameter("glitch_intensity", BossPhaseIntensity[idx]);
            }
        }
        else if (!IsBoss)
        {
            // Leicht erhöhte Intensität im Overload-Zustand
            float intensity = isOverload ? NormalIntensity * 1.5f : NormalIntensity;
            _shaderMat.SetShaderParameter("glitch_intensity", intensity);
        }
    }

    private MeshInstance3D FindMesh(Node node)
    {
        if (node is MeshInstance3D m) return m;
        foreach (Node child in node.GetChildren())
        {
            var result = FindMesh(child);
            if (result != null) return result;
        }
        return null;
    }
}
