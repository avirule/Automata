#version 460 core

layout (location = 0) in int vert;
layout (location = 1) in int uv;
layout (location = 2) in mat4 model;

layout (std140, binding = 0) uniform camera_uniforms
{
    vec4 _viewport;
    vec4 _params;
    mat4 _proj;
    mat4 _view;
};

out gl_PerVertex { vec4 gl_Position; };

layout (location = 0) out fragment
{
    vec3 uv;
    vec3 color;
    mat4 view;
    mat4 proj;
} fragment;

void main()
{
    vec4 uncompressedPosition =
        vec4(
            (vert >> 0) & 63,
            (vert >> 6) & 63,
            (vert >> 12) & 63,
            1.0
        );

    ivec3 uncompressedNormal =
        ivec3(
            ((vert >> 18) & 3) - 1,
            ((vert >> 20) & 3) - 1,
            ((vert >> 22) & 3) - 1
        );
    ivec3 uncompressedUV =
        ivec3(
            (uv >> 0) & 63,
            (uv >> 6) & 63,
            (uv >> 12) & 63
        );

    vec3 lerpedNormal =
        vec3(
            smoothstep(-1.5, 4.25, uncompressedNormal.x) * 1.3,
            smoothstep(-1.5, 4.25, uncompressedNormal.y) * 1.15,
            smoothstep(-1.5, 4.25, uncompressedNormal.z)
        );

    fragment.uv = uncompressedUV;
    fragment.color = vec3(lerpedNormal.x + lerpedNormal.y + lerpedNormal.z);
    fragment.view = _view;
    fragment.proj = _proj;
    gl_Position = (_proj * _view * model) * uncompressedPosition;
}