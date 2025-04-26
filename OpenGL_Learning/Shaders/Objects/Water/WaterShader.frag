#version 460 core 

uniform sampler2D texture0;
uniform float time;

in vec2 texCoord;
in vec3 vecPos;

out vec4 FragColor;


void main()  
{ 
	float speed = 0.002;
	float scale = 16;

	float waterOpacity = 0.5;
	float foamOpacity = 1.f;

	vec4 textureColor = (texture(texture0, vec2(texCoord.y + time * speed, 
										texCoord.x + time * speed) * scale) + 
				 		texture(texture0, vec2(texCoord.y - time * speed,
										texCoord.x + time * speed) * scale)) / 2;

	float blendFactor = clamp(vecPos.y * 2, 0, 1);
	textureColor.a = mix(waterOpacity, foamOpacity, blendFactor);

	vec4 foamColor = mix(vec4(0, 0, 1, waterOpacity), vec4(1, 1, 1, foamOpacity), blendFactor);

	FragColor = textureColor * mix( vec4(1, 1, 1, foamOpacity), foamColor, 0.5 );
} 