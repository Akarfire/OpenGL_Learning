#version 460 core 

// Uniforms
uniform vec3 light_direction;
uniform float ambient_light;
uniform mat4 object_rotation;

// Inputs
in vec2 texCoord;
in vec3 normal;

// Outputs
out vec4 FragColor;


void main()  
{ 
	vec3 worldNormal = normalize( mat3(object_rotation) * normal);
	
    // Sky color
	vec4 skyColor1 = vec4(0.3, 0.67, 1.0, 1.0);
    vec4 skyColor2 = vec4(0.0, 0.19, 0.89, 1.0);
    float skyColorBlendFactor = clamp(dot(vec3(0, 1, 0), worldNormal), 0.0, 1.0);

    vec4 skyColor = mix(skyColor1, skyColor2, skyColorBlendFactor);

    // Sun mask and color
    float sunMask = pow(clamp(dot(light_direction, worldNormal), 0.0, 1.0), 100);

    vec4 sunColorCrown = vec4(1.0, 0.7, 0.0, 1.0);
    vec4 sunColor = mix(sunColorCrown, vec4(1), pow(sunMask, 0.2));


    FragColor = mix(skyColor, sunColor, sunMask);
} 