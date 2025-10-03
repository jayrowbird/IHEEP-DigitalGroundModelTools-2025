// Basic Texture Shader

//type vertex
#version 450 core

layout(location = 0) in vec3 a_Position; 
layout(location = 1) in vec4 a_Color; 
layout(location = 2) in int Id;
 
 uniform mat4 u_ViewProjection;
 layout(location = 0) out vec4 fragcolor;
 out flat int uId;
  
void main()
{ 
	gl_Position = u_ViewProjection * vec4(a_Position, 1.0);
	fragcolor = a_Color; 
	uId = Id;
} 