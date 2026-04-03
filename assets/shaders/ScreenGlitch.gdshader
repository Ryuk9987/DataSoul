shader_type canvas_item;
render_mode unshaded, blend_mix;

// Screen-Overlay-Glitch für Cutscene und Gameplay-Idle
// Läuft auf einem transparenten ColorRect (Color=0,0,0,0)
// Bei glitch_intensity=0 → komplett transparent (unsichtbar)

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float glitch_speed : hint_range(0.0, 20.0) = 5.0;

float rand(float x) {
    return fract(sin(x * 127.1) * 43758.5453);
}

void fragment() {
    float t = TIME * glitch_speed;

    // Horizontale Störstreifen
    float band_y = floor(UV.y * 16.0 + t * 0.4);
    float trigger = step(0.90 - glitch_intensity * 0.3, rand(band_y + floor(t * 0.5)));
    float h_shift = (rand(band_y * 3.7 + t) * 2.0 - 1.0) * glitch_intensity * 0.15 * trigger;

    // Weißblitz-Linie
    float line_trigger = step(0.985 - glitch_intensity * 0.1, rand(floor(t * 2.0) + 9.0));
    float line_y = rand(floor(t * 2.0) + 99.0);
    float line_dist = abs(UV.y - line_y);
    float line_flash = line_trigger * smoothstep(0.005, 0.0, line_dist) * glitch_intensity;

    // RGB-Shift sichtbar als farbiger Overlay
    float r = h_shift * glitch_intensity;
    float b_val = -h_shift * glitch_intensity * 0.5;

    float alpha = glitch_intensity * (trigger * 0.25 + line_flash * 0.5);
    alpha = clamp(alpha, 0.0, 0.85);

    COLOR = vec4(r + line_flash, line_flash * 0.2, b_val + line_flash * 0.2, alpha);
}
