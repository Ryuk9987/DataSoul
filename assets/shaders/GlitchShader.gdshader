shader_type spatial;
render_mode cull_disabled;

uniform vec4 albedo_color : source_color = vec4(0.8, 0.1, 0.1, 1.0);
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.3;
uniform float glitch_speed : hint_range(0.0, 10.0) = 2.0;
uniform float emission_strength : hint_range(0.0, 3.0) = 0.5;
uniform bool overload_mode = false;

float rand(float x) {
    return fract(sin(x * 127.1) * 43758.5453);
}

void vertex() {
    float t = TIME * glitch_speed;
    // Zeitscheiben-basierter Glitch: nur bestimmte Y-Bänder verschieben
    float band = floor(VERTEX.y * 6.0 + t * 0.7);
    float trigger = step(0.85, rand(band + floor(t)));
    float offset = (rand(band * 7.3) * 2.0 - 1.0) * glitch_intensity * trigger;
    VERTEX.x += offset;

    // Overload: stärkeres Zittern + Y-Verzerrung
    if (overload_mode) {
        float ov = sin(TIME * 12.0 + VERTEX.y * 20.0) * glitch_intensity * 0.5;
        VERTEX.x += ov;
        VERTEX.z += ov * 0.4;
    }
}

void fragment() {
    float t = TIME * glitch_speed;
    vec3 col = albedo_color.rgb;

    // RGB-Shift bei Glitch
    float band = floor(UV.y * 8.0 + t * 0.5);
    float trigger = step(0.88, rand(band + floor(t * 0.3)));
    if (trigger > 0.5) {
        col.r = mix(col.r, rand(UV.y + t), 0.4);
        col.b = mix(col.b, rand(UV.y + t + 1.0), 0.3);
    }

    // Overload: rotes Flackern
    if (overload_mode) {
        float flicker = sin(TIME * 18.0) * 0.5 + 0.5;
        col = mix(col, vec3(1.0, 0.0, 0.0), flicker * 0.4);
    }

    ALBEDO = col;
    EMISSION = col * emission_strength;
    ROUGHNESS = 0.6;
    METALLIC = 0.2;
}
