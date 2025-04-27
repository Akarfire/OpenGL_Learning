#version 460 core

// Layout
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

// Uniforms
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float time;
uniform vec3 object_location;
uniform vec3 camera_location;

// Outputs
out vec2 texCoord;
out vec3 vecPos;
out float distanceFalloff;

float GetWaterHeight(vec3 worldPos, float time)
{
    float wave1 = sin(dot(worldPos.xz / 25, vec2(0.3, 0.7)) * 4.0 + time * 1.2);
    float wave2 = sin(dot(worldPos.xz / 25, vec2(0.8, -0.6)) * 6.0 + time * 1.5);
    float wave3 = sin(dot(worldPos.xz / 25, vec2(-0.5, 0.5)) * 10.0 + time * 2.0);

    float height = (wave1 + wave2 * 0.5 + wave3 * 0.25) * 0.5; // Combine and scale
    return 2 * height;
}

void main()
{
	float scale = 3;
	float scaleX = 1;
	float scaleZ = 4;
	float speed = 1;

	vec3 worldPos = aPosition + object_location;
	float cameraDistanceSqr = dot(worldPos - camera_location, worldPos - camera_location);

	float distanceFactor = 0.5 * cameraDistanceSqr / 5000;

	// sin((worldPos.x) / 4 + time * 0.5)
	vec3 newPos = aPosition + 1 * vec3(0, GetWaterHeight(worldPos, time * 0.5) * mix((1 - clamp(pow(distanceFactor, 2), 0, 1)), 1.0, 0.5), 0) - vec3(0, distanceFactor, 0);
	
	gl_Position = vec4(newPos, 1.0) * model * view * projection;
	
	
	texCoord = vec2(worldPos.x, worldPos.z) / 256;
	vecPos = newPos;
	distanceFalloff = distanceFactor;
}