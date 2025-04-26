#version 460 core

// Layout
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

// Uniforms
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float time;

// Outputs
out vec2 texCoord;
out vec3 vecPos;


void main()
{
	float scale = 3;
	float scaleX = 1;
	float scaleZ = 4;
	float speed = 1;

	vec3 newPos = aPosition + 1 * vec3(0, sin(aPosition.x / 4 + time * 0.5), 0);
	
	gl_Position = vec4(newPos, 1.0) * model * view * projection;
	
	texCoord = aTexCoord;
	vecPos = newPos;
}