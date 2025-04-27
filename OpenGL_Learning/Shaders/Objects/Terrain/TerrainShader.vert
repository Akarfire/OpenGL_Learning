#version 460 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

uniform vec3 camera_location;
uniform vec3 object_location;

out vec2 texCoord;
out vec3 normal;
out vec3 pos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec3 worldPos = aPosition + object_location;

    float cameraDistanceSqr = dot(worldPos - camera_location, worldPos - camera_location);

    float falloffFactor = cameraDistanceSqr / 3000;

    vec4 position = vec4(aPosition
    - vec3(0, falloffFactor, 0)
    , 1.0) * model * view * projection;
	gl_Position = position;

	texCoord = aTexCoord;
	normal = aNormal;
    pos = vec3(vec4(aPosition, 1.0) * model);
}

