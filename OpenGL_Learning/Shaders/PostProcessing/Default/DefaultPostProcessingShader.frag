#version 460 core

// Uniforms
uniform sampler2D texture0; // Scene color
uniform sampler2D texture1; // Scene depth
uniform float time;

// Inputs
in vec2 texCoord;

// Outputs
out vec4 FragColor;

void main()  
{ 
	FragColor = texture(texture0, texCoord);
} 