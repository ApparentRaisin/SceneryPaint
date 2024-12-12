Shader "Unlit/NormalsReplacement"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 screenNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float rand : TEXCOORD3;
            };

            sampler2D _ClipTex;
            float4 _ClipTex_ST;
            float _Cutoff;

            float hash13(float3 p3)
            {
	            p3  = frac(p3 * .1031);
                p3 += dot(p3, p3.zyx + 31.32);
                return frac((p3.x + p3.y) * p3.z);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ClipTex);
                o.screenNormal = mul(UNITY_MATRIX_V,UnityObjectToWorldNormal(v.normal));
                o.screenPos = ComputeScreenPos(o.vertex);
                //rand value constructed from world position used to distingush objects
                o.rand = 1;//(hash13(mul(unity_ObjectToWorld,float4(0,0,0,1)))*0.5+0.5) * 0.9 + 0.1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the clip texture for cutout
                fixed4 clipTex = tex2D(_ClipTex, i.uv);
                clip(clipTex - _Cutoff);
                //return screenspace normal remapped to 0-1 anr rand value used to distingush objects
                return float4(i.screenNormal * 0.5 + 0.5, i.vertex.z);//;
            }
            ENDCG
        }
    }
}
