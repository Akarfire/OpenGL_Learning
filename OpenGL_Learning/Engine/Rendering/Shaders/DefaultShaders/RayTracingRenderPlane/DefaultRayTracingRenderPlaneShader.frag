#version 460 core

// Uniforms
uniform sampler2D texture0; // Scene color
uniform float time;
uniform float screen_width;
uniform float screen_height;

// Inputs
in vec2 texCoord;

// Outputs
out vec4 FragColor;

void main()  
{ 
	//FragColor = texture(texture0, texCoord);

	vec2 inverseResolution = 1.0 / vec2(screen_width, screen_height);

    // Sample the current pixel and neighbors
    vec3 colorCenter = texture(texture0, texCoord).rgb;
    vec3 colorUp     = texture(texture0, texCoord + vec2(0.0, inverseResolution.y)).rgb;
    vec3 colorDown   = texture(texture0, texCoord - vec2(0.0, inverseResolution.y)).rgb;
    vec3 colorLeft   = texture(texture0, texCoord - vec2(inverseResolution.x, 0.0)).rgb;
    vec3 colorRight  = texture(texture0, texCoord + vec2(inverseResolution.x, 0.0)).rgb;

//    // Compute simple edge detection
//    vec3 edgeH = abs(colorLeft - colorRight);
//    vec3 edgeV = abs(colorUp - colorDown);
//    float edgeStrength = max(max(edgeH.r, edgeV.r), max(edgeH.g, edgeV.g));
//
//    // Lerp between original and blurred based on edge strength
//    vec3 average = (colorLeft + colorRight + colorUp + colorDown) * 0.25;
//    float blendFactor = smoothstep(0.1, 0.4, edgeStrength); // tweak these for sharpness

    //vec3 finalColor = mix(colorCenter, average, 0.25);
    vec3 averageColor = (colorLeft + colorRight + colorUp + colorDown + colorCenter) / 5;
    vec3 minColor = min(colorLeft, min(colorRight, min( colorUp, min(colorDown, colorCenter))));
    vec3 maxColor = max(colorLeft, max(colorRight, max( colorUp, max(colorDown, colorCenter))));

    vec3 finalColor = mix(mix(colorCenter, averageColor, 0.5), minColor, 0.5);

    FragColor = vec4(finalColor, 1.0);
} 