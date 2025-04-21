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



#define M_PI 3.14159265358979323846

float rand(vec2 co){return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);}
float rand (vec2 co, float l) {return rand(vec2(rand(co), l));}
float rand (vec2 co, float l, float t) {return rand(vec2(rand(co, l), t));}

float perlin(vec2 p, float dim, float time) {
	vec2 pos = floor(p * dim);
	vec2 posx = pos + vec2(1.0, 0.0);
	vec2 posy = pos + vec2(0.0, 1.0);
	vec2 posxy = pos + vec2(1.0);
	
	float c = rand(pos, dim, time);
	float cx = rand(posx, dim, time);
	float cy = rand(posy, dim, time);
	float cxy = rand(posxy, dim, time);
	
	vec2 d = fract(p * dim);
	d = -0.5 * cos(d * M_PI) + 0.5;
	
	float ccx = mix(c, cx, d.x);
	float cycxy = mix(cy, cxy, d.x);
	float center = mix(ccx, cycxy, d.y);
	
	return center * 2.0 - 1.0;
}

// p must be normalized!
float perlin(vec2 p, float dim) {
	
	/*vec2 pos = floor(p * dim);
	vec2 posx = pos + vec2(1.0, 0.0);
	vec2 posy = pos + vec2(0.0, 1.0);
	vec2 posxy = pos + vec2(1.0);
	
	// For exclusively black/white noise
	/*float c = step(rand(pos, dim), 0.5);
	float cx = step(rand(posx, dim), 0.5);
	float cy = step(rand(posy, dim), 0.5);
	float cxy = step(rand(posxy, dim), 0.5);*/
	
	/*float c = rand(pos, dim);
	float cx = rand(posx, dim);
	float cy = rand(posy, dim);
	float cxy = rand(posxy, dim);
	
	vec2 d = fract(p * dim);
	d = -0.5 * cos(d * M_PI) + 0.5;
	
	float ccx = mix(c, cx, d.x);
	float cycxy = mix(cy, cxy, d.x);
	float center = mix(ccx, cycxy, d.y);
	
	return center * 2.0 - 1.0;*/
	return perlin(p, dim, 0.0);
}


void main()
{
	float scale = 3;
	float scaleX = 1;
	float scaleZ = 4;
	float speed = 1;
	vec3 newPos = aPosition + 0.2 * vec3(0, pow(
					sin(aPosition.x / (scale * scaleX) + time) + 0.25 * sin(aPosition.z / (scale * scaleZ) + time) + 0.1 * sin(aPosition.x / (scale * 2) * (-1) + time) + 0.1 * sin(aPosition.z / (scale * 2) * (-1) + time),
					2), 0);

	newPos = aPosition + 0.6 * sin(aPosition.x / (scale * scaleX * 4) + time * speed * 0.5) *
		vec3(	0, 
				perlin( vec2(aPosition.x / (scale * scaleX) + time * speed, aPosition.z / (scale * scaleZ) + 0.1 * time * speed), 1)
				, 0);

	newPos = aPosition + 1 * vec3(0, sin(aPosition.x / 4 + time * 0.5), 0);
	
	
	gl_Position = vec4(newPos, 1.0) * model * view * projection;
	
	texCoord = aTexCoord;
	vecPos = newPos;
	stime = time;
}