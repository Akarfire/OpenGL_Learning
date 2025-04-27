#version 460 core 

// Uniforms
uniform vec3 light_direction;
uniform float ambient_light;
uniform mat4 object_rotation;
uniform float time;

// Inputs
in vec2 texCoord;
in vec3 normal;
in vec3 pos;

// Outputs
out vec4 FragColor;


void main()  
{ 
	vec3 lightDir = light_direction;
	vec3 worldNormal = normalize( mat3(object_rotation) * normal);
	
    float slope = pow(1 - abs(dot(worldNormal, vec3(0, 1, 0))), 0.5);

    vec4 sandColor = vec4(0.94, 0.8, 0.44, 1.0);
    vec4 rockColor = vec4(0.47, 0.47, 0.47, 1.0);
    vec4 grassColor = vec4(0.25, 0.65, 0.25, 1.0);

    vec4 grassSand = mix(grassColor, sandColor, float(pos.y < 0));
    vec4 landColor = mix(grassSand, rockColor, float(slope > 0.35));

	FragColor = landColor * clamp( dot(worldNormal, lightDir) + ambient_light, 0, 1 );
	FragColor.a = 1;
} 