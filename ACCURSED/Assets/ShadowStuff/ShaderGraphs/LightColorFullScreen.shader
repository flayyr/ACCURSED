Shader "Custom/LightColorFullScreen"
{
    Properties
    {
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "LightColorFullScreen"

            Cull Off
            ZWrite Off
            ZTest Always
            Blend One Zero

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Light
            {
                float2 position;

                float depth;

                float radius;

                float intensity;

                float3 color;
            };

            StructuredBuffer<Light> _Lights;

            int _LightCount;

            float2 _CameraMin;
            float2 _CameraSize;

            //-----------------------------------------

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float2 pos;

                switch (input.vertexID)
                {
                    case 0:
                        pos = float2(-1,-1);
                        break;

                    case 1:
                        pos = float2(-1,3);
                        break;

                    default:
                        pos = float2(3,-1);
                        break;
                }

                output.positionCS = float4(pos,0,1);

                output.uv = pos * 0.5 + 0.5;

                return output;
            }

            //-----------------------------------------

            float3 EvaluateLight(Light light, float2 worldPos)
            {
                float2 delta = worldPos - light.position;

                float distSq = dot(delta, delta);

                float radiusSq = light.radius * light.radius;

                if (distSq > radiusSq)
                    return 0;

                float distance = sqrt(distSq);

                float attenuation = saturate(1.0 - distance / light.radius);

                // attenuation = pow(
                //     attenuation,
                //     light.falloff);

                return
                    light.color.rgb *
                    attenuation *
                    light.intensity;
            }

            //-----------------------------------------

            float3 CalculateLighting(float2 worldPos)
            {
                float3 lighting = 0;

                for(int i = 0; i < _LightCount; i++)
                {
                    lighting += EvaluateLight(
                        _Lights[i],
                        worldPos);
                }

                return lighting;
            }

            //-----------------------------------------

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                uv.y = 1.0 - uv.y;

                float2 worldPos =
                    _CameraMin +
                    uv * _CameraSize;

                float3 lighting =
                    CalculateLighting(worldPos);

                return half4(lighting,1);
            }

            ENDHLSL
        }
    }
}