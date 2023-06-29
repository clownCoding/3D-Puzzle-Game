Shader "Custom/AlphaMy"
{
    Properties
    {
        _BaseColorMap("BaseMap",2D) = "white"{}
        _BaseColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _MainTex1("Albedo1 (RGB)", 2D) = "white" {}
        _MainTex2("Albedo (RGB)", 2D) = "white" {}
        _MainTex3("Albedo (RGB)", 2D) = "white" {}
        
        _Noise("Noise",2D) = "white"{}
        _Cutoff("Cutoff",Range(0.01,1.1)) = 1
        _lineIntensity("MylineIn",Range(0,1.5)) = 0.37
        _Dis("_Dis",Range(0,1)) = 0.19
        _Power("Power",float)=1
        _Scale("Scale",float)=1
        [HDR]_BurnColor("EmiColor",Color) = (2.5,1,1,1)//灼烧光颜色
        [HDR]_LineColor("EmiColor",Color) = (2.5,1,1,1)
    }
        SubShader
        {
            Tags {  "RenderPipeline"="UniversalRenderPipeline" "IgnoreProjector"="True" "RenderType"="Transparent" "Queue"="Transparent"}
            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma multi_compile _ _ONE
            #pragma multi_compile _ _TWO
            #pragma multi_compile _ _THREE
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _MainTex1_ST;
            float4 _MainTex2_ST;
            float4 _MainTex3_ST;
            float4 _BaseColorMap_ST;
            float4 _Noise_ST;
            float4 _BaseColor;
            float _Cutoff;
            float _lineIntensity;
            real4 _BurnColor;
            real4 _LineColor;
            float _Dis;
            float _Power;
            float _Scale;
            CBUFFER_END
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MainTex1);
            SAMPLER(sampler_MainTex1);
            TEXTURE2D(_MainTex2);
            SAMPLER(sampler_MainTex2);
            TEXTURE2D(_MainTex3);
            SAMPLER(sampler_MainTex3);
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);
            TEXTURE2D(_BaseColorMap);
            SAMPLER(sampler_BaseColorMap);
            float4 GrayChange(float4 input) {
                float a = 0.3 * input.r + input.g * 0.59 + input.b * 0.11;
                return float4(a,a,a,input.a);
            };
            struct a2v {
                float4 positionOS:POSITION;
                float4 normalOS:NORMAL;
                float2 texcoord:TEXCOORD;
            };
            struct v2f {
                float4 positionCS:SV_POSITION;
                float2 uv:TEXCOORD;
                float2 texcoord1:TEXCOORD1;
                float2 texcoord2:TEXCOORD2;
                float2 texcoord3:TEXCOORD3;

            };
            ENDHLSL


            pass {
                Tags{"LightMode" = "UniversalForward"}
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite off
                HLSLPROGRAM
                #pragma vertex VERT
                #pragma fragment FRAG
                v2f VERT(a2v i)
                {
                    v2f o;
                    o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                    
                    
                    o.texcoord2 = TRANSFORM_TEX(i.texcoord,_Noise);
                    o.texcoord3 = TRANSFORM_TEX(i.texcoord,_BaseColorMap);
                    o.uv = TRANSFORM_TEX(i.texcoord,_MainTex);

                    return o;
                }
                float4 FRAG(v2f i) :SV_TARGET
                {

                    float4 texx = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
                    float4 te = GrayChange(texx);

                    #if _ONE
                    texx = SAMPLE_TEXTURE2D(_MainTex1,sampler_MainTex1,i.uv);
                    te = GrayChange(texx);
                    #endif
                    #if _TWO
                    texx = SAMPLE_TEXTURE2D(_MainTex2,sampler_MainTex2,i.uv);
                    te = GrayChange(texx);
                    #endif
                    #if _THREE
                    texx = SAMPLE_TEXTURE2D(_MainTex3,sampler_MainTex2,i.uv);
                    te = GrayChange(texx);
                    #endif


                    float3 tex = float3(te.a,te.a,te.a);
                    
                    float4 albedo = SAMPLE_TEXTURE2D(_BaseColorMap,sampler_BaseColorMap,i.texcoord3);
                    float Alpha = texx.w;
                    float AA = lerp(1,0,step(Alpha,0));
                    half4 noise = SAMPLE_TEXTURE2D(_Noise,sampler_Noise,i.texcoord2);
                    noise = GrayChange(noise);
                    float3 Emi = pow(abs(noise.rgb),_Power)*_Scale;
                    AA = AA* saturate(Emi.r);
                    float CC = noise.r - _Dis;
                    AA = saturate(AA *CC);
                    float Xiao = _lineIntensity/noise.r;
                    float3 fin = albedo.rgb * Emi * _BurnColor.rgb;
                    float Alp = lerp(AA,AA+0.025,step(AA,0));
                    float3 ww = lerp(fin,_LineColor,step(Alp,AA));
                    return float4(ww,Alp);
                }
                ENDHLSL
            }
        }

}
