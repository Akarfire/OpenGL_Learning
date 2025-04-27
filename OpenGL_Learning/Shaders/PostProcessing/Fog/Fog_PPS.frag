#version 460 core

// Uniforms
uniform sampler2D texture0; // Scene color
uniform sampler2D texture1; // Scene depth
uniform float time;
uniform mat4 projection_inverse;
uniform mat4 view_inverse;
uniform float screen_width;
uniform float screen_height;
uniform mat4 projection;
uniform vec3 camera_location;

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

// //Reconstructs world position from fragment position and depth
// vec3 GetWorldPosition(float depth)
// {
//     // Convert fragment coordinates to normalized device coordinates (NDC)
//     vec4 ndc = vec4(
//         texCoord.x * 2.0 - 1.0,
//         texCoord.y * 2.0 - 1.0,
//         depth * 2.0 - 1.0,
//         1.0
//     );

//     // Transform NDC to view space
//     vec4 viewPos = projection_inverse * ndc;
//     viewPos /= viewPos.w;  // Perspective division

//     // Transform view space to world space
//     vec4 worldPos = view_inverse * viewPos;
    
//     return worldPos.xyz;
// }


//Calculates exponential Height Fog color
vec4 ExponentialHeightFog(vec4 inColor)
{
    // Distant fog settings
    float zNear = 0.01;   // Near distance
    float zFar = 1000.0;  // Far distance
    float zScale = 5.0;

    // Height fog settings
    // float fogStartHeight = 0;
    // float fogEndHeight = 20;

    // Fog color
    vec4 fogColor = vec4(0.4, 0.7, 1.0, 1.0); 

    // Depth liniarization
    float rawDepth = texture(texture1, texCoord).r;
    float depth = LinearizeDepth(rawDepth, zNear, zFar) / zScale;

    // Calculating distant fog
    float distantFog = clamp(depth, 0.0, 1.0);

    // Calculating height fog
    //vec3 worldPos = GetWorldPosition(rawDepth);

    //float height = worldPos.y;
    //float heightFog = clamp((height - fogStartHeight) / (fogEndHeight - fogStartHeight), 0.0, 1.0);

    // Resulting color
    vec4 color = mix(inColor, fogColor, distantFog * 0.6 );

    //return vec4(normalize(worldPos), 1) * color.b;
    return color;
}

float GetWaterHeight(vec3 worldPos, float time)
{
    float wave1 = sin(dot(worldPos.xz / 25, vec2(0.3, 0.7)) * 4.0 + time * 1.2);
    float wave2 = sin(dot(worldPos.xz / 25, vec2(0.8, -0.6)) * 6.0 + time * 1.5);
    float wave3 = sin(dot(worldPos.xz / 25, vec2(-0.5, 0.5)) * 10.0 + time * 2.0);

    float height = (wave1 + wave2 * 0.5 + wave3 * 0.25) * 0.5; // Combine and scale
    return 2 * height;
}

vec4 UnderWaterEffect(vec4 sceneColor)
{
    vec4 underWaterColor = vec4(0.15, 0.49, 0.91, 1.0);
    float underWaterColorEffect = 0.6;

    float waterLevel = GetWaterHeight(camera_location, time * 0.5);
    
    float underWaterMask = float(camera_location.y < waterLevel + 0.1);

    vec4 color = mix(sceneColor, mix(sceneColor, underWaterColor, underWaterColorEffect), underWaterMask );

    return color;
}


// -- MAIN --

void main()  
{ 
    // Samplig scene color
    vec4 sceneColor = texture(texture0, texCoord);

    // Output
    FragColor = UnderWaterEffect(ExponentialHeightFog(sceneColor));
}


