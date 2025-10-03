//type fragment
#version 450 core

layout(location = 0) in vec4 fragcolor;
in flat int uId;

layout(location = 0) out vec4 color; 
layout(location = 1) out int guid;
 
void main()
{
    color = fragcolor; 
    guid = uId;
}
 