shader_type spatial;

uniform vec4 albedo_color : source_color = vec4(0.8, 0.1, 0.1, 1.0);
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.3;
uniform float glitch_speed : hint_range(0.0, 10.0) = 2.0;

void vertex() {
    float glitch = sin(TIME * glitch_speed + VERTEX.y * 10.0) * glitch_intensity;
    VERTEX.x += glitch * step(0.95, sin(TIME * 3.0 + VERTEX.y));
}

void fragment() {
    ALBEDO = albedo_color.rgb;
    EMISSION = albedo_color.rgb * 0.3;
    ROUGHNESS = 0.8;
}
