using Godot;

/// <summary>
/// Factory für Toon/Cel-Shader Materialien — Anime-Style für alle Charaktere.
/// </summary>
public static class ToonMaterial
{
    private static Shader _toonShader;
    private static Shader _outlineShader;

    private static Shader GetToonShader()
    {
        if (_toonShader == null)
            _toonShader = GD.Load<Shader>("res://assets/shaders/toon_character.gdshader");
        return _toonShader;
    }

    private static Shader GetOutlineShader()
    {
        if (_outlineShader == null)
            _outlineShader = GD.Load<Shader>("res://assets/shaders/toon_outline.gdshader");
        return _outlineShader;
    }

    /// <summary>
    /// Erstellt ein Toon-ShaderMaterial mit optionalem Outline.
    /// </summary>
    public static ShaderMaterial Create(Color albedo, Color? shadowColor = null,
        Color? outlineColor = null, float outlineWidth = 0.025f)
    {
        var mat = new ShaderMaterial();
        mat.Shader = GetToonShader();
        mat.SetShaderParameter("albedo", albedo);
        mat.SetShaderParameter("shadow_color", shadowColor ?? albedo.Darkened(0.35f));
        mat.SetShaderParameter("highlight_color", Colors.White);
        return mat;
    }

    /// <summary>
    /// Erstellt das Outline-Material (zweites Surface-Material).
    /// </summary>
    public static ShaderMaterial CreateOutline(Color color, float width = 0.025f)
    {
        var mat = new ShaderMaterial();
        mat.Shader = GetOutlineShader();
        mat.SetShaderParameter("outline_color", color);
        mat.SetShaderParameter("outline_width", width);
        return mat;
    }

    /// <summary>
    /// Wendet Toon + Outline auf ein MeshInstance3D an (braucht mind. 2 Surfaces oder
    /// fügt Overlay hinzu).
    /// </summary>
    public static void Apply(MeshInstance3D mesh, Color albedo,
        Color? outlineColor = null, float outlineWidth = 0.025f)
    {
        if (mesh == null) return;
        mesh.SetSurfaceOverrideMaterial(0, Create(albedo));
        // Outline als MaterialOverlay
        mesh.MaterialOverlay = CreateOutline(outlineColor ?? Colors.Black, outlineWidth);
    }
}
