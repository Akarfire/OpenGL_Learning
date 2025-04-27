#version 460 core 

uniform sampler2D texture0;
uniform float time;

in vec2 texCoord;
in vec3 vecPos;
in float distanceFalloff;

out vec4 FragColor;


void main()  
{ 
	float speed = 0.002;
	float scale = 16;

	float waterOpacity = 0.7;
	float foamOpacity = 1.f;

	vec4 textureColor = (texture(texture0, vec2(texCoord.y + time * speed, 
										texCoord.x + time * speed) * scale) + 
				 		texture(texture0, vec2(texCoord.y - time * speed,
										texCoord.x + time * speed) * scale)) / 2;

	float blendFactor = clamp(0.3 + vecPos.y * 0.7, 0, 1);
	textureColor.a = mix(waterOpacity, foamOpacity, blendFactor);

	vec4 foamColor = mix(vec4(0, 0, 1, waterOpacity), vec4(1, 1, 1, foamOpacity), blendFactor);

	FragColor = textureColor * mix( vec4(1, 1, 1, foamOpacity), foamColor, 0.5 );
	FragColor.a = FragColor.a * (1 - clamp(pow(distanceFalloff, 3) * 0.05, 0, 1));

	//FragColor = vec4(vec3(pow(distanceFalloff, 3)), 1);
	//FragColor = vec4(vec3(0.0, 0.3 + height * 0.7, 0.6), 1.0);
} 