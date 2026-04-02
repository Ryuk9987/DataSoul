shader_type spatial;
render_mode diffuse_toon, specular_toon, cull_back;

uniform vec4 albedo : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 shadow_color : source_color = vec4(0.3, 0.3, 0.45, 1.0);
uniform vec4 highlight_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float shadow_threshold : hint_range(-1.0, 1.0) = 0.1;
uniform float highlight_threshold : hint_range(0.0, 1.0) = 0.85;
uniform float highlight_smoothness : hint_range(0.0, 0.2) = 0.05;

void fragment() {
    ALBEDO = albedo.rgb;
    SPECULAR = 0.0;
    ROUGHNESS = 1.0;
}

void light() {
    float NdotL = dot(NORMAL, LIGHT);

    // Two-tone cel shading
    float shadow = smoothstep(shadow_threshold - 0.05, shadow_threshold + 0.05, NdotL);
    vec3 toon_color = mix(shadow_color.rgb, albedo.rgb, shadow);

    // Highlight rim
    float highlight = smoothstep(highlight_threshold - highlight_smoothness,
                                  highlight_threshold + highlight_smoothness, NdotL);
    toon_color = mix(toon_color, highlight_color.rgb, highlight * 0.3);

    DIFFUSE_LIGHT += toon_color * ATTENUATION * LIGHT_COLOR;
}
