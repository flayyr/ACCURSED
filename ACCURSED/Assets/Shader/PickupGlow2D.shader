Shader "Custom/PickupGlow2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR] _BottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
        [HDR] _TopColor ("Top Color", Color) = (0.65, 0.5, 1, 1)

        _Intensity ("Intensity", Range(0, 5)) = 1.5

        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha One
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _BottomColor;
                float4 _TopColor;

                float _Intensity;
                float _PulseSpeed;
                float _PulseAmount;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;
                output.color = input.color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D( _MainTex, sampler_MainTex, input.uv);
                
                half4 gradientColor = lerp( _BottomColor, _TopColor, input.uv.y);

                // Subtle breathing effect.
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed + input.uv.y * 4.0) * _PulseAmount;

                half alpha = tex.a * gradientColor.a * input.color.a;

                half3 rgb = tex.rgb * gradientColor.rgb * input.color.rgb * _Intensity * pulse;

                return half4(rgb, alpha);
            }

            ENDHLSL
        }
    }
}