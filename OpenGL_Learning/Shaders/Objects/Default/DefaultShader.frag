#version 460 core 

// Uniforms
uniform sampler2D texture0;
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
	vec3 lightDir = light_direction;
	vec3 worldNormal = normalize( mat3(object_rotation) * normal);
	
	FragColor = texture(texture0, texCoord); //* clamp( dot(worldNormal, lightDir) + ambient_light, 0, 1 );
	FragColor.a = 1;
} 