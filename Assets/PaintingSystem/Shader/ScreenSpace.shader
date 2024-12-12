Shader "Unlit/ScreenSpace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TXCOORD1;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4x4 _WorldToCameraO;
            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                //calculate screenspace position based on Main Camera
                o.screenPos = ComputeScreenPos(UnityObjectToClipPos(v.vertex));

                //layout mesh verts accourding to uv2 coords (lightmap) for capture by orthor camera
                float4 sc = mul(_WorldToCameraO,float4(((v.uv2) * 2 - 1), 0.3, 1));
                sc.xy *= ((_ScreenParams.x)/_ScreenParams.yx) * LinearEyeDepth(sc.w);
                o.vertex = mul(UNITY_MATRIX_P,sc);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // collect the screen position and depth
                float2 screenPos = i.screenPos.xy/i.screenPos.w;
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture,screenPos).r);
                depth +=  LinearEyeDepth(tex2D(_CameraDepthTexture,screenPos + float2(1,0) * _CameraDepthTexture_TexelSize.xy * 0.5).r);
                depth +=  LinearEyeDepth(tex2D(_CameraDepthTexture,screenPos + float2(-1,0) * _CameraDepthTexture_TexelSize.xy * 0.5).r);
                depth +=  LinearEyeDepth(tex2D(_CameraDepthTexture,screenPos + float2(0,1) * _CameraDepthTexture_TexelSize.xy * 0.5).r);
                depth +=  LinearEyeDepth(tex2D(_CameraDepthTexture,screenPos + float2(0,-1) * _CameraDepthTexture_TexelSize.xy * 0.5).r);
                depth /= 5.;
                
                //return 0 blue value when behind other objects OR outside of 0-1 screen range
                fixed4 col = float4(screenPos,(((depth) - i.screenPos.w) > -0.5) * (i.screenPos.w>0.3) * (screenPos.x > 0) * (screenPos.x < 1) * (screenPos.y > 0) * (screenPos.y < 1),1);
                
                return col;
            }
            ENDCG
        }

        
    }

    
}
