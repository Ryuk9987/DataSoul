shader_type canvas_item;
render_mode unshaded;

// Screen-Overlay-Glitch für Cutscene und Gameplay-Idle
// Nur für den Spieler sichtbar (CanvasLayer, nicht im 3D-Raum)
// .hack-Prinzip: NPCs reagieren nie darauf

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float glitch_speed : hint_range(0.0, 20.0) = 5.0;
uniform float aberration_strength : hint_range(0.0, 0.05) = 0.005;
uniform bool scan_lines = false;

float rand(float x) {
    return fract(sin(x * 127.1) * 43758.5453);
}

void fragment() {
    if (glitch_intensity <= 0.001) {
        COLOR = texture(TEXTURE, UV);
        return;
    }

    float t = TIME * glitch_speed;
    vec2 uv = UV;

    // Horizontal-Shift in Bändern (digitaler Störstreifen)
    float band_y = floor(uv.y * 16.0 + t * 0.4);
    float trigger = step(0.90 - glitch_intensity * 0.3, rand(band_y + floor(t * 0.5)));
    float h_shift = (rand(band_y * 3.7 + t) * 2.0 - 1.0) * glitch_intensity * 0.04 * trigger;
    uv.x = clamp(uv.x + h_shift, 0.0, 1.0);

    // Vertikaler Sprung (selten, nur bei hoher Intensität)
    float v_trigger = step(0.97 - glitch_intensity * 0.15, rand(floor(t * 0.3) + 1.5));
    float v_shift = (rand(floor(t * 0.3) * 5.1) * 2.0 - 1.0) * glitch_intensity * 0.015 * v_trigger;
    uv.y = clamp(uv.y + v_shift, 0.0, 1.0);

    // RGB-Chromatic Aberration
    float aber = aberration_strength * glitch_intensity;
    float r = texture(TEXTURE, clamp(uv + vec2( aber, 0.0), 0.0, 1.0)).r;
    float g = texture(TEXTURE, uv).g;
    float b = texture(TEXTURE, clamp(uv - vec2( aber, 0.0), 0.0, 1.0)).b;
    float a = texture(TEXTURE, uv).a;

    // Scan-Lines (optional, sehr subtil)
    float scan = 1.0;
    if (scan_lines) {
        scan = 1.0 - 0.06 * mod(floor(UV.y * 300.0), 2.0);
    }

    // Kurze Weißblitz-Linie (simuliert Frame-Glitch)
    float line_trigger = step(0.985 - glitch_intensity * 0.1, rand(floor(t * 2.0) + 9.0));
    float line_y = rand(floor(t * 2.0) + 99.0);
    float line_dist = abs(uv.y - line_y);
    float line_flash = line_trigger * smoothstep(0.005, 0.0, line_dist) * glitch_intensity;

    vec3 col = vec3(r, g, b) * scan;
    col += vec3(line_flash * 0.8);

    COLOR = vec4(col, a);
}
