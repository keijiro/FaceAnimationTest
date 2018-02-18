Shader "Hidden/DrawBuffer"
{
    SubShader
    {
        Cull Off

        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:Vertex addshadow nolightmap
        #pragma target 5.0

        #include "Common.hlsl"

        #ifdef SHADER_AVAILABLE_COMPUTE
        StructuredBuffer<TriangleData> _TriangleBuffer;
        #endif

        struct Attributes
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            uint vertexID : SV_VertexID;
        };

        struct Input { float dummy; };

        void Vertex(inout Attributes v)
        {
        #ifdef SHADER_AVAILABLE_COMPUTE
            uint pidx = v.vertexID / 3;
            uint vidx = v.vertexID - pidx * 3;

            v.vertex = _TriangleBuffer[pidx].vertices[vidx];
            v.texcoord = _TriangleBuffer[pidx].uvs[vidx];
            v.normal = _TriangleBuffer[pidx].normal;
        #endif
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = 1;
            o.Metallic = 0;
            o.Smoothness = 0;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
