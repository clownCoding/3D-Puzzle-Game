Shader "Custom/SSS"
{
    Properties
    {
        _MainTex("MainTex",2D) = "white"{}
        _DeapTex("DeapTex",2D) = "white"{}
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Sdistortion("Subsurface distortion Intenity",Range(0,4))= 0.0
        _Power("Power" ,Range(0.01,20)) = 0.0
        _Scale("Scale",Range(0.01,1)) = 0.1
        _FLightAttnuation("FLightAttnuation",Range(0.01,20))=1
        _EPower("EPower",Range(0.01,20))=0.02
        _EScale("EScale",Range(0.01,20))=0.1
        _EColor ("EColor", Color) = (1,1,1,1)
        _Speed("Speed",Range(0,2))= 1

        _RimLightWidth("Rim Light Width",range(0.0,2))=0.006
        _RimLightThreshold("Rim Light Threshold",range(-0.9,0.5))=0
        _RimLightIntensity("Rim Light Intensity",range(0.0,2))=0.25
        [HDR]_RimColor ("RimColor", Color) = (1,1,1,1)
        _Lerp("Lerp",float) = 1

    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline"}
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "HSV2RGB.hlsl"

        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT 

        CBUFFER_START(UnityPerMatrial)
        float4 _BaseColor;
        float4 _EColor;
        float _Glossiness;
        float _Metallic;
        float _Sdistortion;
        float _Scale;
        float _Power;
        float _Speed;
        float _EPower;
        float _EScale;
        float _FLightAttnuation;
        float4 _MainTex_ST;
        float4 _DeapTex_ST;
        float _RimLightWidth;
        float _RimLightThreshold;
        float _RimLightIntensity;
        float4 _RimColor;
        float _Lerp;

        CBUFFER_END
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DeapTex);
        SAMPLER(sampler_DeapTex);
        TEXTURE2D_X_FLOAT(_CameraDepthTexture); 
        SAMPLER(sampler_CameraDepthTexture);
        struct a2v
        {
            float4 positionOS:POSITION;
            float4 normalOS:NORMAL;
            float2 texcoord:TEXCOORD;
            float4 tangentOS: TANGENT;
            //float2 lightmapUV:TEXCOORD2;
        };
        struct v2f
        {
            float4 positionCS:SV_POSITION;
            float4 texcoord:TEXCOORD;
            float4 normalWS:NORMAL;
            float4 tangentWS:TANGENT;
            float4 BtangentWS: TEXCOORD1;
        };
        ENDHLSL

        

        pass{
            Tags{"LightMode"="UniversalForward" "RenderType"="Opaque"}

            HLSLPROGRAM
            #pragma vertex VERT
            #pragma fragment FRAG
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITION_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            float4 GrayChange(float4 input)
            {
                float a = 0.3*input.r + input.g*0.59 + input.b*0.11;
                return float4(a,a,a,input.a);
            };
            float3 RGB2HSV(float3 rgb)
            {
                float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
                // 比较r和max(b,g)
                float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
                return hsv;

            };

            v2f VERT(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                //i.texcoord.x += _Time.x*_Speed;
                //i.texcoord.y += _Time.x*_Speed;
                o.texcoord.xy = TRANSFORM_TEX(i.texcoord,_MainTex);
                
                o.normalWS.xyz = normalize(TransformObjectToWorldNormal(i.normalOS.xyz));
                o.tangentWS.xyz = normalize(TransformObjectToWorldDir(i.tangentOS.xyz));
                o.BtangentWS.xyz = cross(o.normalWS.xyz,o.tangentWS.xyz) * i.tangentOS.w*unity_WorldTransformParams.w;
                float3 posWS = TransformObjectToWorld(i.positionOS);
                o.normalWS.w=posWS.x;
                o.tangentWS.w=posWS.y;
                o.BtangentWS.w=posWS.z;
                return o;
            }

            half4 FRAG(v2f v):SV_TARGET
            {
                float3 WSpos = float3(v.normalWS.w,v.tangentWS.w,v.BtangentWS.w);
                Light myLight = GetAdditionalLight(0,WSpos);
                //Light myLight = GetMainLight();
                float3 N = v.normalWS;
                
                float3 L=normalize(myLight.direction);
                float3 V=SafeNormalize(_WorldSpaceCameraPos-WSpos);
                float3 H=normalize(V+L);
                float NdotV=max(saturate(dot(N,V)),0.000001);//不取0 避免除以0的计算错误
                float NdotL=max(saturate(dot(N,L)),0.000001);
                float HdotV=max(saturate(dot(H,V)),0.000001);
                float NdotH=max(saturate(dot(H,N)),0.000001);
                float LdotH=max(saturate(dot(H,L)),0.000001);
                float LdotV=max(saturate(dot(L,V)),0.000001);
                //float PI = 3.1415926;
                //直光
                //计算Specular D
                float D2 = _Glossiness*_Glossiness;
                float demon = (NdotH*NdotH)*(D2-1)+1;
                float SD_down = demon * demon ;//* PI;
                //SD_down *= pow(LdotH,2) * (_Glossiness + 0.5) * 4.0;
                float Spe_D = D2/SD_down;
                //计算Specular G
                float kd = pow((_Glossiness+1),2)*0.125;
                
                float G1 = NdotV/lerp(NdotV,1,kd);//NdotV*(1-kd)+kd;
                float G2 = NdotL/lerp(NdotL,1,kd);
                float Spe_G = G1 * G2;
                //计算 Specular F
                float3 F0 = float3(0.05, 0.05, 0.05);
                
                //lerp(0.04,_BaseColor,_Metallic);//float3(0.05, 0.05, 0.05);//(0.24, 0.24, 0.24)
                float3 Spe_F = F0 + (1-F0)*pow((1-LdotH),5);
                //计算BRDF
                float3 BRDFSpeSection=Spe_D*Spe_F*Spe_G/(4*NdotL*NdotV);
                float3 DirectSpeColor=BRDFSpeSection*NdotL*PI*myLight.color;
                //漫反射
                
                float3 KD=(1-Spe_F)*(1-_Metallic);
                float2 run = float2(v.texcoord.x+_Time.x*_Speed,v.texcoord.y-_Time.x*_Speed);
                float4 Albedo = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,v.texcoord.xy);
                float4 Deapth = SAMPLE_TEXTURE2D(_DeapTex,sampler_DeapTex,run);
                float3 DirDiffuse = KD*_BaseColor.rgb*Albedo.xyz;//NdotL;
                

                //背光
                /*float3 back = -L+N*_Sdistortion;
                float3 tt = saturate(dot(N,back));
                float3 I = saturate(pow(tt,_Power))*_Scale*myLight.color;*/

                float3 back = L + N*_Sdistortion;
                float3 tt = saturate(dot(N,-back));
                float3 flDot = pow(tt,_Power)*_Scale;
                float3 flt = (flDot + _GlossyEnvironmentColor.rgb) * Deapth.rgb * _FLightAttnuation;

          

                float3 DirectionColor=DirectSpeColor + DirDiffuse*flt;
                //float3 xx=DirectionColor + I;
                //自发光
                float4 gay = GrayChange(Albedo);
                float Emis = pow(gay,_EPower)*_EScale*clamp(max(_SinTime,_CosTime),0.1,2).x;
                float3 Ecolor = clamp(Emis,0,16)*_EColor;
                DirectionColor += Ecolor;

                //边缘光
                float width = smoothstep(_RimLightWidth,_RimLightThreshold,NdotV)*_RimLightIntensity;
                
                float4 rimLight = width * _RimColor;
                float3 RimCol = lerp(rimLight.rgb,DirectionColor.rgb,_Lerp);

                //float3 outCol = max(RimCol,DirectionColor.rgb);
                DirectionColor += rimLight.rgb;
                float3 outCol = clamp(DirectionColor,0,16);
                return float4(outCol,1);//Emis,Emis,Emis


            }
            ENDHLSL

        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
}
