Shader "Custom/LargeLaserFighterBodyShaderURP"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _PanelTex("Panel (RGB)", 2D) = "white" {}
        _CanopyTex("Canopy (RGB)", 2D) = "white" {}

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _TopColor("TopColor", Color) = (1,1,1,1)
        _BottomColor("BottomColor", Color) = (1,1,1,1)

        _CanopyRatio("CanopyRatio", Range(0,1)) = 0.0

        _Dimensions("Plane Dimensions", Vector) = (1,1,1,0)
        _TintRatios("Tint Ratios", Vector) = (1,1,1,0)

        _NoseTint("Nose Tint", Color) = (1,1,1,1)
        _WingTint("Wing Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 posOS : TEXCOORD2;
                float3 posWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            TEXTURE2D(_PanelTex);      SAMPLER(sampler_PanelTex);
            TEXTURE2D(_CanopyTex);     SAMPLER(sampler_CanopyTex);

            float4 _MainTex_ST;
            float _Glossiness;
            float _Metallic;
            float _CanopyRatio;
            float4 _TopColor;
            float4 _BottomColor;
            float3 _Dimensions;
            float3 _TintRatios;
            float3 _NoseTint;
            float3 _WingTint;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.posOS = IN.positionOS.xyz;
                OUT.posWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 pos = IN.posOS;

                float zPerc = clamp((pos.z + (_Dimensions.z * 0.5)) / _Dimensions.z, 0, 1);
                float xPerc = clamp(abs(pos.x) / _Dimensions.x, 0, 1);
                float y = pos.y;

                float2 mainUV = float2((pos.x - y - 8) * 0.07, (pos.z + y + 5) * 0.1);
                float2 panelUV = float2((pos.x + y * 0.5) * 0.3, (pos.z + y * 0.5) * 0.3);

                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV);
                float4 panelCol = SAMPLE_TEXTURE2D(_PanelTex, sampler_PanelTex, panelUV);
                float4 canopyCol = SAMPLE_TEXTURE2D(_CanopyTex, sampler_CanopyTex, IN.uv);

                float d = dot(normalize(IN.normalWS), float3(0, 1, 0));
                d = (d + 1) * 0.5;

                float4 color = baseCol;
                color = lerp(_BottomColor, color, d);
                color.rgb = lerp(color.rgb, _NoseTint, step(_TintRatios.z, zPerc));
                color.rgb = lerp(color.rgb, _WingTint, step(_TintRatios.x, xPerc));
                color *= panelCol;
                color *= lerp(float4(1,1,1,1), canopyCol, _CanopyRatio);

                // Setup surface data
                SurfaceData surfaceData;
                surfaceData.albedo = color.rgb;
                surfaceData.alpha = color.a;
                surfaceData.metallic = _Metallic;
                surfaceData.specular = 0;
                surfaceData.smoothness = _Glossiness;
                surfaceData.normalTS = float3(0, 0, 1);
                surfaceData.occlusion = 1;
                surfaceData.emission = 0;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                InputData inputData;
                inputData.positionWS = IN.posWS;
                inputData.normalWS = normalize(IN.normalWS);
                inputData.viewDirectionWS = SafeNormalize(_WorldSpaceCameraPos - IN.posWS);
                inputData.shadowCoord = float4(0, 0, 0, 0);
                inputData.fogCoord = 0;
                inputData.vertexLighting = float3(0, 0, 0);
                inputData.bakedGI = float3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = 0;
                inputData.shadowMask = 1;

                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
