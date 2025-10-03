// Vertex shader:
// ================
#version 330 core
layout (location = 0) in vec3 a_Position;
layout(location = 0) in vec3 a_Normal; 
layout(location = 1) in vec4 a_Color; 
layout(location = 2) in int Id;

out vec3 LightingColor; // resulting color from lighting calculations

uniform vec3 lightPos;
uniform vec3 viewPos;
uniform vec3 lightColor;

uniform mat4 u_ViewProjection;
layout(location = 0) out vec4 fragcolor;
out flat int uId;

void main()
{
    gl_Position = u_ViewProjection * vec4(a_Position, 1.0);
	fragcolor = a_Color; 
	uId = Id;

    // gouraud shading
    // ------------------------
    //vec3 Position = vec3(model * vec4(aPos, 1.0));
    vec3 Normal = mat3(transpose(inverse(model))) * a_Normal;
    
    // ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;
  	
    // diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - a_Position);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    
    // specular
    float specularStrength = 1.0; // this is set higher to better show the effect of Gouraud shading 
    vec3 viewDir = normalize(viewPos - a_Position);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;      

    LightingColor = ambient + diffuse + specular;
}