#version 460 core 

in vec2 texCoord;
in vec3 vecPos;
in float stime;

out vec4 FragColor;

uniform sampler2D texture0;

void main()  
{ 
	float speed = 0.005;
	float scale = 4;
	vec4 textureColor = (texture(texture0, vec2(texCoord.y + stime * speed, 
										texCoord.x + stime * speed) * scale) + 
				 texture(texture0, vec2(texCoord.y - stime * speed,
										texCoord.x + stime * speed) * scale)) / 2;

	vec4 foamColor = mix(vec4(0, 0, 1, 1), vec4(1, 1, 1, 1), clamp(vecPos.y * 2, 0, 1));

	FragColor = textureColor * mix( vec4(1, 1, 1, 1), foamColor, 0.5 );
} 