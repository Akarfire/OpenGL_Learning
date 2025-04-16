#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 vecPos;
out float stime;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float time;

void main()
{
	float scale = 2;
	float scaleX = 1.5;
	float scaleZ = 2;
	vec3 newPos = aPosition + 0.25 * vec3(0, pow(
					sin(aPosition.x / (scale * scaleX) + time) + 0.25 * sin(aPosition.z / (scale * scaleZ) + time) + 0.1 * sin(aPosition.x / (scale * 2) * (-1) + time) + 0.1 * sin(aPosition.z / (scale * 2) * (-1) + time),
					2), 0);
	
	gl_Position = vec4(newPos, 1.0) * model * view * projection;
	
	texCoord = aTexCoord;
	vecPos = newPos;
	stime = time;
}