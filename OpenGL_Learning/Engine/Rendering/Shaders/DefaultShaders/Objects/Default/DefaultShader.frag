#version 460 core 

// Uniforms
uniform sampler2D texture0;
uniform vec3 light_direction;
uniform float ambient_light;
uniform mat4 object_rotation;
uniform vec3 camera_vector;

// Inputs
in vec2 texCoord;
in vec3 normal;

// Outputs
out vec4 FragColor;


void main()  
{ 
	float specular = 100;
	float specularBlend = 0.1;
	vec4 specularColor = vec4(1.0);

	vec3 worldNormal = normalize( mat3(object_rotation) * normal);

	vec3 specularDir = normalize(light_direction + -1 * camera_vector);
	float specularFactor = clamp(dot(worldNormal, specularDir), 0, 1);
	float specularIntensity = pow(specularFactor, specular);
	
	FragColor = texture(texture0, texCoord) 
		* clamp( dot(worldNormal, light_direction) + ambient_light, 0, 1 )
		+ specularColor * specularIntensity * specularBlend;
	FragColor.a = 1;
} 