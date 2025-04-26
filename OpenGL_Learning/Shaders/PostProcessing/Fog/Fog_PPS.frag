#version 460 core

// Uniforms
uniform sampler2D texture0; // Scene color
uniform sampler2D texture1; // Scene depth
uniform float time;

// Inputs
in vec2 texCoord;

// Outputs
out vec4 FragColor;

// Linearize depth function
float LinearizeDepth(float depth, float near, float far)
{
    float z = depth * 2.0 - 1.0; // back to NDC
    return (2.0 * near * far) / (far + near - z * (far - near));
}

void main()  
{ 
    float zNear = 0.01;   // Near distance
    float zFar = 1000.0;  // Far distance
    float zScale = 10.0;

    // Samplig scene color
    vec4 sceneColor = texture(texture0, texCoord);

    // Depth liniarization
    float depth = LinearizeDepth(texture(texture1, texCoord).r, zNear, zFar) / zScale;

    // Fog color
    vec4 fogColor = vec4(0.0, 0.6, 1.0, 1.0);

    // Output
    FragColor = mix(sceneColor, fogColor, clamp(depth, 0, 1));
} 