Shader "Custom/SSS"
{
    Properties
    {
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Sdistortion("Subsurface distortion Intenity",Range(0,1))= 0.0
        _Power("Power" ,Range(0,20)) = 0.0
        _Scale("Scale",Range(0.01,5)) = 0.1
        _LightPower("LightPower",Range(0.01,1))=0.02
    }
    SubShader
    {
        Tags{ "RenderPipeline"="UniversalRenderPipeline"}
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT 

        CBUFFER_START(UnityPerMatrial)
        float4 _BaseColor;
        float _Glossiness;
        float _Metallic;
        float _Sdistortion;
        float _Scale;
        float _Power;
        float _LightPower;
        CBUFFER_END

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

            v2f VERT(a2v i)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
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
                float D2 = pow(_Glossiness,2);
                float SD_down = pow((pow(NdotH,2)*(D2-1)+1),2)*PI;
                float Spe_D = D2/SD_down;
                //计算Specular G
                float kd = pow((_Glossiness+1),2);
                float G1 = NdotV/NdotV*(1-kd)+kd;
                float G2 = NdotL/NdotL*(1-kd)+kd;
                float Spe_G = G1 * G2;
                //计算 Specular F
                float3 F0 = float3(0.24, 0.24, 0.24);
                
                //lerp(0.04,_BaseColor,_Metallic);//float3(0.05, 0.05, 0.05);//(0.24, 0.24, 0.24)
                float3 Spe_F = F0 + (1-F0)*pow((1-LdotH),5);
                //计算BRDF
                float3 BRDFSpeSection=Spe_D*Spe_F*Spe_G/(4*NdotL*NdotV);
                float3 DirectSpeColor=BRDFSpeSection*NdotL*PI;
                //漫反射
                float3 KD=(1-Spe_F)*(1-_Metallic);
                float3 DirDiffuse = KD*_BaseColor.rgb;//*NdotL
                

                //背光
                float3 back = -L+N*_Sdistortion;
                float3 I = saturate(pow((dot(V,back)),_Power))*_Scale*myLight.color;
          
                //float3 DirectionColor=DirectSpeColor + DirDiffuse;

                float3 DirectionColor=DirectSpeColor + DirDiffuse +I;

                return float4(DirectionColor,1);


            }
            ENDHLSL

        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
